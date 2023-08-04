using System.Diagnostics;
using DCA.Extensions.BackgroundTask.Telemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;
using OpenTelemetry.Trace;
using Xunit;
using FluentAssertions;
namespace DCA.Extensions.BackgroundTask.Test;

public class TracingTests
{
    [Fact]
    public async Task TraceParentTest()
    {
        var exportedItems = new List<Activity>();
        var tracer = Sdk.CreateTracerProviderBuilder()
            .AddSource(Tracing.ActivitySource.Name)
            .AddProcessor<TestActivityProcessor>()
            .AddInMemoryExporter(exportedItems)
            .Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddBackgroundTask();
        services.AddScoped<ServiceA>();
        var provider = services.BuildServiceProvider();

        var host = (BackgroundTaskHostedService)provider.GetRequiredService<IHostedService>();
        await host.StartAsync(default);

        var dispatcher = provider.GetRequiredService<IBackgroundTaskDispatcher>();

        var currentActivity = Tracing.ActivitySource.StartActivity("test");
        await dispatcher.DispatchAsync<ServiceA>(async dep =>
        {
            await Task.Delay(5 * 1000);
            await dep.DoWork();
        });
        currentActivity?.Dispose();

        await host.StopAsync(default);
        WaitForActivityExport(exportedItems, 2);

        exportedItems[0].DisplayName.Should().Be("test");
        exportedItems[1].DisplayName.Should().Be("Execute Background Task");
        exportedItems[1].ParentSpanId.Should().Be(exportedItems[0].SpanId);
    }

    private static void WaitForActivityExport(List<Activity> exportedItems, int count)
    {
        Assert.True(SpinWait.SpinUntil(
            () =>
            {
                Thread.Sleep(10);
                return exportedItems.Count >= count;
            },
            TimeSpan.FromSeconds(5)));
    }
}

public class TestActivityProcessor : BaseProcessor<Activity>
{
    public override void OnStart(Activity activity)
    {
        Console.WriteLine($"OnStart: {activity.DisplayName}");
    }

    public override void OnEnd(Activity activity)
    {
        Console.WriteLine($"OnEnd: {activity.DisplayName}");
    }
}
