using Azure.Identity;

using Consumer.API.EventHandlers;

using Microsoft.Extensions.Azure;

using Platform.Config;

var builder = WebApplication.CreateBuilder(args);

PlatformOptions platformOptions = new();

var platformOptionSection = builder.Configuration.GetSection(PlatformOptions.Key);
platformOptionSection.Bind(platformOptions);

builder.Services.Configure<PlatformOptions>(platformOptionSection);
builder.Services.Configure<PlatformServiceBusOptions>(
    builder.Configuration.GetSection(
        $"{PlatformOptions.Key}:{PlatformServiceBusOptions.Key}"));

builder.Services
    .AddAzureClients(azClientBuilder =>
    {
        azClientBuilder
            .AddServiceBusClient(platformOptions.ServiceBus.ConnectionString.SampleApp)
            .ConfigureOptions(options =>
            {
                options.RetryOptions = new()
                {
                    Delay = TimeSpan.FromSeconds(5)
                };
            })
            .WithName(platformOptions.ServiceBus.Namespace.SampleApp);
    });

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.RegisterEventHandlers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

var orderCreatedQueueMessageHandler = app.Services.GetService<OrderMessageHandler>();

if (orderCreatedQueueMessageHandler is not null)
{
    orderCreatedQueueMessageHandler.StartProcessing();
}

var fulfilmentTopicSubscription = app.Services.GetService<FulfilmentHandler>();

if (fulfilmentTopicSubscription is not null)
{
    fulfilmentTopicSubscription.StartProcessing();
}

var deliveryTopicSubscription = app.Services.GetService<DeliveryHandler>();

if (deliveryTopicSubscription is not null)
{
    deliveryTopicSubscription.StartProcessing();
}


app.Run();
