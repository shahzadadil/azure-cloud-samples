namespace Cloud.Azure.ServiceBus;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using global::Azure.Messaging.ServiceBus;

using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;

/// <summary>
/// Send messages to service bus queue or topic. Inherit from this and add details. Implement the handler.
/// </summary>
/// <typeparam name="TMessage">The type of message to be sent</typeparam>
public abstract class ServiceBusMessageSender<TMessage> : IAsyncDisposable
{
    private readonly IAzureClientFactory<ServiceBusClient> _ServiceBusClientFactory;

    private ServiceBusClient _ServiceBusClient;
    private ServiceBusSender _ServiceBusSender;

    protected readonly ILogger<ServiceBusMessageSender<TMessage>> _Logger;

    protected abstract string Namespace { get; }
    protected abstract string QueueOrTopicName { get; }

    public ServiceBusMessageSender(
        IAzureClientFactory<ServiceBusClient> serviceBusClientFactory,
        ILogger<ServiceBusMessageSender<TMessage>> logger)
    {
        _Logger = logger;
        _ServiceBusClientFactory = serviceBusClientFactory;
    }

    private void Initialise()
    {
        _ServiceBusClient = _ServiceBusClientFactory.CreateClient(Namespace);

        if (_ServiceBusClient is null)
        {
            throw new ServiceBusException(
                $"Error creating service bus client from factory for namespace ({Namespace})",
                ServiceBusFailureReason.GeneralError);
        }

        _ServiceBusSender = _ServiceBusClient.CreateSender(QueueOrTopicName);

        if (_ServiceBusSender is null)
        {
            throw new ServiceBusException(
                $"Error creating service bus processor from client for queue/topic ({QueueOrTopicName})",
                ServiceBusFailureReason.GeneralError);
        }
    }

    public virtual async Task SendMessageAsync(TMessage message)
    {
        Initialise();
        ServiceBusMessage serviceBusMessage = new(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message)));
        await _ServiceBusSender.SendMessageAsync(serviceBusMessage);
    }

    public virtual async Task SchdeuleMessageAsync(TMessage message, DateTimeOffset scheduleOffset)
    {
        Initialise();

        ServiceBusMessage serviceBusMessage = new(
            Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message)))
            {
                ScheduledEnqueueTime = scheduleOffset
            };

        await _ServiceBusSender.SendMessageAsync(serviceBusMessage);
    }

    public async ValueTask DisposeAsync()
    {
        await _ServiceBusSender.DisposeAsync();
    }
}
