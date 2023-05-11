namespace Cloud.Azure.ServiceBus;
using System.Threading.Tasks;

using global::Azure.Messaging.ServiceBus;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Platform.Config;

public class ServiceBusMessageProcessor : IAsyncDisposable
{
    private readonly ILogger _Logger;
    private readonly IServiceBusMessageHandler _ServicBusMessageHandler;
    private readonly ServiceBusClientOptions _ServiceBusClientOptions;
    private readonly PlatformServiceBusOptions _ServiceBusOptions;

    private ServiceBusClient _ServiceBusClient;
    private ServiceBusProcessor _ServiceBusProcessor;

    public ServiceBusMessageProcessor(
        IOptions<PlatformServiceBusOptions> platformServiceBusOptions,
        IServiceBusMessageHandler messageHandler,
        ILogger<ServiceBusMessageProcessor> logger)
    {
        _ServiceBusOptions = platformServiceBusOptions.Value;
        _ServicBusMessageHandler = messageHandler;
        _Logger = logger;
        _ServiceBusClientOptions = new ServiceBusClientOptions();
    }

    public async Task Start()
    {
        _ServiceBusClient = new ServiceBusClient(
            _ServiceBusOptions.ConnectionString.SampleApp,
            _ServiceBusClientOptions);

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
        await _ServicBusMessageHandler.HandleAsync(message);
        await arg.CompleteMessageAsync(arg.Message);
    }

    public async ValueTask DisposeAsync()
    {
        await _ServiceBusProcessor.StopProcessingAsync();
        await _ServiceBusProcessor.DisposeAsync();
        await _ServiceBusClient.DisposeAsync();
    }
}
