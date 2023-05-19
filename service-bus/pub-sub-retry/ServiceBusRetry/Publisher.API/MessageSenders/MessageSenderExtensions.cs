namespace Publisher.API.MessageSenders;
public static class MessageSenderExtensions
{
    public static void RegisterMessageSenders(this IServiceCollection services)
    {
        services.AddSingleton<OrderCreatedMessageSender>();
        services.AddSingleton<OrderCreatedMessagePublisher>();
    }
}
