using System.Diagnostics.Metrics;

namespace DCA.Extensions.BackgroundTask.Telemetry;
internal static class Metrics
{
    public const string MeterName = "DCA.DotNetToolkit";
    private static readonly Meter s_meter = new(MeterName, "1.0.0");
    public static UpDownCounter<int> CounterInflightBackgroundTasks { get; }
        = s_meter.CreateUpDownCounter<int>("background_tasks_inflight");

    public static Counter<int> CounterDispatchedTasks { get; }
        = s_meter.CreateCounter<int>("background_tasks_dispatched");

    public static Counter<int> CounterProcessedTasks { get; }
        = s_meter.CreateCounter<int>("background_tasks_processed");
}
