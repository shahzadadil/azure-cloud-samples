using Azure.Identity;

using Cloud.Azure.ServiceBus;

using Consumer.API.EventHandlers;

using Microsoft.Extensions.Azure;

using Platform.Config;

var builder = WebApplication.CreateBuilder(args);
PlatformOptions platformOptions = new();

var platformOptionSection = builder.Configuration.GetSection(PlatformOptions.Key);
platformOptionSection.Bind(platformOptions);

builder.Services.Configure<PlatformOptions>(platformOptionSection);
builder.Services.Configure<PlatformServiceBusOptions>(builder.Configuration.GetSection($"{PlatformOptions.Key}:{PlatformServiceBusOptions.Key}"));

builder.Services
    .AddAzureClients(azClientBuilder =>
    {
        azClientBuilder
            .AddServiceBusClient(platformOptions.ServiceBus.ConnectionString.SampleApp)
            .WithCredential(new DefaultAzureCredential())
            .WithName(platformOptions.ServiceBus.Namespace.SampleApp);
    });

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddSingleton<OrderMessageHandler>();
//builder.Services.AddSingleton<ServiceBusQueueMessageProcessor>();
//builder.Services.AddTransient<ServiceBusTopicMessageProcessor>();
//builder.Services.AddSingleton<FulfilmentHandler>();
//builder.Services.AddSingleton<DeliveryHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

var orderCreatedQueueMessageHandler = app.Services.GetService<OrderMessageHandler>();

if (orderCreatedQueueMessageHandler is not null)
{
    orderCreatedQueueMessageHandler.StartProcessing();
}

//var fulfilmentTopicSubscription = app.Services.GetService<ServiceBusTopicMessageProcessor>();

//if (fulfilmentTopicSubscription is not null)
//{
//    await fulfilmentTopicSubscription.Start(
//        platformOptions.ServiceBus.Subscriptions.Fulfilment,
//        app.Services.GetService<FulfilmentHandler>());
//}

//var deliveryTopicSubscription = app.Services.GetService<ServiceBusTopicMessageProcessor>();

//if (deliveryTopicSubscription is not null)
//{
//    await deliveryTopicSubscription.Start(
//        platformOptions.ServiceBus.Subscriptions.Delivery,
//        app.Services.GetService<DeliveryHandler>());
//}

app.Run();
