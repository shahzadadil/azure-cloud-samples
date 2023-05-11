using Cloud.Azure.ServiceBus;

using Consumer.API;

using Platform.Config;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<PlatformOptions>(builder.Configuration.GetSection(PlatformOptions.Key));
builder.Services.Configure<PlatformServiceBusOptions>(builder.Configuration.GetSection($"{PlatformOptions.Key}:{PlatformServiceBusOptions.Key}"));

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddSingleton<IServiceBusMessageHandler, OrderMessageHandler>();
builder.Services.AddSingleton<ServiceBusMessageProcessor>();

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

var processor = app.Services.GetService<ServiceBusMessageProcessor>();
await processor.Start();

app.Run();
