namespace Cloud.Azure.ServiceBus;
using System.Threading.Tasks;

using global::Azure.Messaging.ServiceBus;

using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Platform.Config;

public class ServiceBusQueueMessageProcessor : IAsyncDisposable
{
    private readonly ILogger _Logger;
    private readonly IServiceBusMessageHandler _ServicBusMessageHandler;
    private readonly PlatformServiceBusOptions _ServiceBusOptions;
    private readonly IAzureClientFactory<ServiceBusClient> _ServiceBusClientFactory;

    private ServiceBusClient _ServiceBusClient;
    private ServiceBusProcessor _ServiceBusProcessor;

    public ServiceBusQueueMessageProcessor(
        IAzureClientFactory<ServiceBusClient> serviceBusClientFactory,
        IOptions<PlatformServiceBusOptions> platformServiceBusOptions,
        IServiceBusMessageHandler messageHandler,
        ILogger<ServiceBusQueueMessageProcessor> logger)
    {
        _ServiceBusClientFactory = serviceBusClientFactory;
        _ServiceBusOptions = platformServiceBusOptions.Value;
        _ServicBusMessageHandler = messageHandler;
        _Logger = logger;
    }

    public async Task Start()
    {
        _ServiceBusClient = _ServiceBusClientFactory.CreateClient("SampleAppServiceBusClient");

        _ServiceBusProcessor = _ServiceBusClient.CreateProcessor(
            _ServiceBusOptions.QueueName.OrderCreated,
            new ServiceBusProcessorOptions());

        _ServiceBusProcessor.ProcessMessageAsync += MessageHandler;
        _ServiceBusProcessor.ProcessErrorAsync += ErrorHandler;

        await _ServiceBusProcessor.StartProcessingAsync();
    }

    private Task ErrorHandler(ProcessErrorEventArgs arg)
    {
        _Logger.LogError(arg.Exception, "Error processing queue message");
        return Task.FromResult(-1);
    }

    private async Task MessageHandler(ProcessMessageEventArgs arg)
    {
        var message = arg.Message;
        _ServicBusMessageHandler.Handle(message);
        await arg.CompleteMessageAsync(arg.Message);
    }

    public async ValueTask DisposeAsync()
    {
        await _ServiceBusProcessor.StopProcessingAsync();
        await _ServiceBusProcessor.DisposeAsync();
        await _ServiceBusClient.DisposeAsync();
    }
}

//public abstract class MessageProcessor
//{
//    public async Task StartListening()
//    {
//        _ServiceBusClient = _ServiceBusClientFactory.CreateClient("SampleAppServiceBusClient");

//        _ServiceBusProcessor = _ServiceBusClient.CreateProcessor(
//            _ServiceBusOptions.QueueName.OrderCreated,
//            new ServiceBusProcessorOptions());

//        _ServiceBusProcessor.ProcessMessageAsync += MessageHandler;
//        _ServiceBusProcessor.ProcessErrorAsync += ErrorHandler;

//        await _ServiceBusProcessor.StartProcessingAsync();
//    }
//}
