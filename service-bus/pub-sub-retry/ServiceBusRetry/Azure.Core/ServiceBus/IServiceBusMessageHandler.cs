namespace Cloud.Azure.ServiceBus;
using System.Threading.Tasks;

using global::Azure.Messaging.ServiceBus;

public interface IServiceBusMessageHandler
{
    Task HandleAsync(ServiceBusReceivedMessage message);
}
