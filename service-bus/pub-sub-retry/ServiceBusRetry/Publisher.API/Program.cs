using Azure.Identity;

using Microsoft.Extensions.Azure;

using Platform.Config;

using Publisher.API.MessageSenders;

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
            .WithCredential(new DefaultAzureCredential())
            .WithName(platformOptions.ServiceBus.Namespace.SampleApp);
    });

builder.Services.RegisterMessageSenders();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();


app.UseAuthorization();

app.MapControllers();

app.Run();
