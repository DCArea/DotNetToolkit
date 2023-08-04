using System.Diagnostics;
using DCA.Extensions.BackgroundTask.Telemetry;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using static System.Console;

namespace DCA.Extensions.BackgroundTask.PerfTest;

public class PerfTest
{
    public static async Task RunAsync()
    {
        int producer = 10;
        int repeat = 10000000;

        using var meterProvider = Sdk.CreateMeterProviderBuilder()
            .AddMeter(Metrics.MeterName, "System.Runtime")
            .AddConsoleExporter()
            .Build();

        Counter.Init();

        var services = new ServiceCollection().AddLogging();
        services.AddBackgroundTask(opt =>
        {
            opt.Channels.Add(new BackgroundTaskChannelOptions{
                Key = "MyChannelKey",
                Capacity = 1024, // Max number of inflight tasks in the channel, A negetive number means unlimited
                WorkerCount = 1 // Number of workers for the channel, it should be 1 if all tasks in the channel are started immediately when dispatching
            });
        });
        var sp = services.BuildServiceProvider();
        var pool = (BackgroundTaskHostedService)sp.GetServices<IHostedService>().Single(svc => svc is BackgroundTaskHostedService);
        await pool.StartAsync(default);
        var dispatcher = sp.GetRequiredService<IBackgroundTaskDispatcher>();

        var sw = Stopwatch.StartNew();
        var dispatchTasks = Enumerable.Range(1, producer)
            .Select(i => Task.Run(() => DispatchFor(dispatcher, repeat)));
        await Task.WhenAll(dispatchTasks);
        sw.Stop();
        WriteLine($"Dispatched {producer * repeat} in {sw.Elapsed.TotalSeconds}s");


        sw.Restart();
        await pool.StopAsync(default);
        sw.Stop();
        WriteLine($"Processed in {sw.Elapsed.TotalSeconds}s");

        static async Task DispatchFor(IBackgroundTaskDispatcher dispatcher, int repeat)
        {
            for (int i = 0; i < repeat; i++)
                await dispatcher.DispatchAsync(
                    ctx => ValueTask.CompletedTask,
                    new PerfTestContext()
                );
        }

    }
}

public record PerfTestContext() : IBackgroundTaskContext;
