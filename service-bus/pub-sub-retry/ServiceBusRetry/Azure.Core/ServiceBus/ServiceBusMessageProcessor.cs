namespace Cloud.Azure.ServiceBus;
using System;
using System.Text.Json;
using System.Threading.Tasks;

using global::Azure.Messaging.ServiceBus;

using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;

public abstract class ServiceBusMessageProcessor<TMessage> : IAsyncDisposable
{
    private readonly IAzureClientFactory<ServiceBusClient> _ServiceBusClientFactory;
    
    private ServiceBusClient _ServiceBusClient;
    private ServiceBusProcessor _ServiceBusProcessor;

    protected readonly ILogger<ServiceBusMessageProcessor<TMessage>> _Logger;

    protected abstract string Namespace { get; }
    protected abstract string QueueOrTopicName { get; }
    protected virtual string Subscription { get; }

    public ServiceBusMessageProcessor(
        IAzureClientFactory<ServiceBusClient> serviceBusClientFactory,
        ILogger<ServiceBusMessageProcessor<TMessage>> logger)
    {
        _Logger = logger;
        _ServiceBusClientFactory = serviceBusClientFactory;
    }

    // This is a handler to be implemented by consumers and handle message as per them
    protected abstract Task HandleMessageAsync(TMessage message);

    public virtual void StartProcessing()
    {
        _ServiceBusClient = _ServiceBusClientFactory.CreateClient(Namespace);

        if (_ServiceBusClient is null)
        {
            throw new ServiceBusException(
                "Error creating service bus client from factory",
                ServiceBusFailureReason.GeneralError);
        }

        _ServiceBusProcessor = string.IsNullOrWhiteSpace(Subscription)
            ? _ServiceBusClient.CreateProcessor(QueueOrTopicName)
            : _ServiceBusClient.CreateProcessor(QueueOrTopicName, Subscription);

        if (_ServiceBusProcessor is null)
        {
            throw new ServiceBusException(
                "Error creating service bus processor from client",
                ServiceBusFailureReason.GeneralError);
        }

        _ServiceBusProcessor.ProcessMessageAsync += ProcessMessageAsync;
        _ServiceBusProcessor.ProcessErrorAsync += ProcessErrorAsync;

        _ServiceBusProcessor.StartProcessingAsync();
    }

    protected virtual Task ProcessErrorAsync(ProcessErrorEventArgs arg)
    {
        _Logger.LogError(arg.Exception, $"Error processing service bus message");
        return Task.CompletedTask;
    }

    private async Task ProcessMessageAsync(ProcessMessageEventArgs arg)
    {
        var serviceBusMessage = arg.Message ?? throw new InvalidOperationException("Empty message received");
        var message = JsonSerializer.Deserialize<TMessage>(serviceBusMessage.Body.ToString());

        if (message is null)
        {
            throw new JsonException("Error deserialising message to object");
        }

        _Logger.LogTrace($"Message received: {serviceBusMessage}");

        await HandleMessageAsync(message);
    }

    public async ValueTask DisposeAsync()
    {
        await _ServiceBusProcessor.StopProcessingAsync();
        await _ServiceBusProcessor.DisposeAsync();
    }
}
