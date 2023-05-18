namespace Cloud.Azure.ServiceBus;
using System.Threading.Tasks;

using global::Azure.Messaging.ServiceBus;

using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Platform.Config;

public class ServiceBusTopicMessageProcessor : IAsyncDisposable
{
    private readonly ILogger _Logger;
    private readonly ServiceBusClientOptions _ServiceBusClientOptions;
    private readonly PlatformServiceBusOptions _ServiceBusOptions;
    private readonly IAzureClientFactory<ServiceBusClient> _ServiceBusClientFactory;

    private ServiceBusClient _ServiceBusClient;
    private ServiceBusProcessor _ServiceBusProcessor;
    private IServiceBusMessageHandler _ServicBusMessageHandler;

    public ServiceBusTopicMessageProcessor(
        IAzureClientFactory<ServiceBusClient> serviceBusClientFactory,
        IOptions<PlatformServiceBusOptions> platformServiceBusOptions,
        ILogger<ServiceBusQueueMessageProcessor> logger)
    {
        _ServiceBusClientFactory = serviceBusClientFactory;
        _ServiceBusOptions = platformServiceBusOptions.Value;
        _Logger = logger;

        _ServiceBusClientOptions = new ServiceBusClientOptions
        {
            RetryOptions = _ServiceBusOptions.RetryOptions
        };
    }

    public async Task Start(string subscription, IServiceBusMessageHandler messageHandler)
    {
        _ServiceBusClient = _ServiceBusClientFactory.CreateClient("SampleAppServiceBusClient");

        _ServiceBusProcessor = _ServiceBusClient.CreateProcessor(
            _ServiceBusOptions.TopicName.OrderCreated,
            subscription,
            new ServiceBusProcessorOptions());

        _ServicBusMessageHandler = messageHandler;

        _ServiceBusProcessor.ProcessMessageAsync += MessageHandler;
        _ServiceBusProcessor.ProcessErrorAsync += ErrorHandler;

        await _ServiceBusProcessor.StartProcessingAsync();
    }

    private Task ErrorHandler(ProcessErrorEventArgs arg)
    {
        _Logger.LogError(arg.Exception, "Error processing topic message");
        return Task.FromResult(-1);
    }

    private async Task MessageHandler(ProcessMessageEventArgs arg)
    {
        var message = arg.Message;

        try
        {
            await _ServicBusMessageHandler.HandleAsync(message);
            await arg.CompleteMessageAsync(arg.Message);
        }
        catch (Exception ex)
        {
            await arg.AbandonMessageAsync(arg.Message);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _ServiceBusProcessor.StopProcessingAsync();
        await _ServiceBusProcessor.DisposeAsync();
        await _ServiceBusClient.DisposeAsync();
    }
}
