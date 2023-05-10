namespace Platform.Config;
public class PlatformOptions
{
    public const string Key = "Platform";

    public ServiceBusOptions ServiceBus { get; set; } = new();
}

public class ServiceBusOptions
{
    public const string Key = "ServiceBus";

    public ServiceBusQueueNameOptions QueueName { get; set; } = new();
    public ServiceBusNamespaceOptions Namespace { get; set; } = new();
    public ServiceBusConnectionStringOptions ConnectionString { get; set; } = new();
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
