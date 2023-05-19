namespace Consumer.API.EventHandlers;

public static class EventHandlerExtensions
{
    public static void RegisterEventHandlers(this IServiceCollection services)
    {
        services.AddSingleton<OrderMessageHandler>();
        services.AddSingleton<FulfilmentHandler>();
        services.AddSingleton<DeliveryHandler>();
    }
}
