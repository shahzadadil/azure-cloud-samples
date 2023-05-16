namespace Platform.Config;

using Azure.Messaging.ServiceBus;

public class PlatformOptions
{
    public const string Key = "Platform";

    public PlatformServiceBusOptions ServiceBus { get; set; } = new();
}

public class PlatformServiceBusOptions
{
    public const string Key = "ServiceBus";

    public ServiceBusQueueNameOptions QueueName { get; set; } = new();
    public ServiceBusNamespaceOptions Namespace { get; set; } = new();
    public ServiceBusConnectionStringOptions ConnectionString { get; set; } = new();
    public ServiceBusRetryOptions RetryOptions { get; set; } = new();
}

public class ServiceBusQueueNameOptions
{
    public const string Key = "QueueName";

    public string OrderCreated { get; set; } = string.Empty;
}

public class ServiceBusNamespaceOptions
{
    public const string Key = "Namespace";

    public string SampleApp { get; set; } = string.Empty;
}

public class ServiceBusConnectionStringOptions
{
    public const string Key = "ConnectionString";

    public string SampleApp { get; set; } = string.Empty;
}
