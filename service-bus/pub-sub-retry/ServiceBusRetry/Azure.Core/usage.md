# Cloud.Azure Project
The project contains the infrstatructure required to operate with Azure cloud. There are hgh level abracted implementations that can be resued with custom implekentations.

## ServiceBus
This namespace consists of  implementations.

### Sender
A service bus message sender. It should be inherited to create your own specific sender. No logic related to interactin with Azure Service bus needs to be written. They are handled. Only inherit the class and use the methods to perform send operations.

### Receiver
A service bus message receiver. It should be inherited to create your own specific receiver. No logic related to interactin with Azure Service bus needs to be written. They are handled. Only inherit the class and implement the custom processing that needs to be handled by specific functionality.

## Steps to Use

### Create infrastructure on Azure
Ceate infra on Azure. A terraform script is already added to the solution. Use that or create the infrastructure manually. All the resources required are listed in the [main.tf](/devops/resources/main.tf) file.

- Create Azure account
- Subscribe to a plan
- Create resource group
- Create Azure Service Bus Namespace
- Create queue
- Create topic, and subscriptions inside the topic

### Create a concrete implementation
Inherit the classes and create your oewn implementation on how to handle specific functionality when a message arrives. Only specify the informations about namespase, queue or topic, subscription (only for topics). For example:

```
public class OrderMessageHandler : ServiceBusMessageProcessor<OrderCreated>
{
    private readonly PlatformServiceBusOptions _ServiceBusOptions;

    public OrderMessageHandler(
        IAzureClientFactory<ServiceBusClient> serviceBusClientFactory,
        ILogger<OrderMessageHandler> logger,
        IOptions<PlatformServiceBusOptions> serviceBusOptions) : base(serviceBusClientFactory, logger)
    {
        _ServiceBusOptions = serviceBusOptions.Value;
    }

    protected override String Namespace => _ServiceBusOptions.Namespace.SampleApp;
    protected override String QueueOrTopicName => _ServiceBusOptions.QueueName.OrderCreated;

    protected override Task HandleMessageAsync(OrderCreated orderCreatedMessage)
    {
        // Handle the message as per need. Throw exception on failures.
    }
}
```

### Setup Azure Service Bus client factory
in your projects where we need to consume the implementation setup the factory to crete service bus clients.

Usually in the Startup or Program class, setup the factory

```
builder.Services
    .AddAzureClients(azClientBuilder =>
    {
        azClientBuilder
            .AddServiceBusClient("<ConnectionString>")
            .WithName("<Namespace>");
    });
```

This will register the service bus client with the provided namespace with the factory.

### Inject Factory and use
To use the servcie bus client for sending or receiving, we would required an implementation like demonsrated above. `IAzureClientFactory<ServiceBusClient>` needs to be injected to create a client instance. Injectinto the constructor and pass to the base class like 

```
public OrderMessageHandler(
        IAzureClientFactory<ServiceBusClient> serviceBusClientFactory,
        ILogger<OrderMessageHandler> logger,
        IOptions<PlatformServiceBusOptions> serviceBusOptions) : base(serviceBusClientFactory, logger)
    {
        _ServiceBusOptions = serviceBusOptions.Value;
    }
```

This was base class will get an instance of the factory and then can use tht to setup the client, sender or receiver.

