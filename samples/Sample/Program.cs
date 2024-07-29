using DCA.Extensions.BackgroundTask;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;


var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddOpenTelemetry()
    .WithTracing(b => b
        .AddSource("DCA.DotNetToolkit")
        .AddAspNetCoreInstrumentation()
        .AddZipkinExporter())
    .WithMetrics(b => b
        .AddMeter("DCA.DotNetToolkit")
        .AddPrometheusExporter());


builder.Services.AddBackgroundTask(opt =>
{
    opt.Channels.First().Capacity = 100;
});
builder.Services.AddScoped<ServiceA>();

var app = builder.Build();
app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.MapPost("/fast-api", async (IBackgroundTaskDispatcher background) =>
{
    for (int i = 0; i < 100; i++)
    {
        await background.DispatchAsync<ServiceA>(async serviceA => await serviceA.DoWork());
    }
    return Results.Ok();
});

app.Run();

public class ServiceA
{
    public async Task DoWork()
    {
        await Task.Delay(60 * 1000);
    }
}