using Azure.Identity;

using Microsoft.Extensions.Azure;

using Platform.Config;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<PlatformOptions>(builder.Configuration.GetSection(PlatformOptions.Key));

builder.Services
    .AddAzureClients(azClientBuilder =>
    {
        PlatformOptions platformOptions = new();
        builder.Configuration.GetSection(PlatformOptions.Key).Bind(platformOptions);

        azClientBuilder
            .AddServiceBusClient(platformOptions.ServiceBus.ConnectionString.SampleApp)
            .WithCredential(new DefaultAzureCredential())
            .WithName("SampleAppServiceBusClient");
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();


app.UseAuthorization();

app.MapControllers();

app.Run();
