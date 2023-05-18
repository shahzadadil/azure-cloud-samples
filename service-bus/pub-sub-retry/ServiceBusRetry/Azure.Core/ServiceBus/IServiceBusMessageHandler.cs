namespace Cloud.Azure.ServiceBus;
using global::Azure.Messaging.ServiceBus;

public interface IServiceBusMessageHandler
{
    void Handle(ServiceBusReceivedMessage message);
}
