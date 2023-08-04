using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Threading;
using DCA.Extensions.BackgroundTask;
using DCA.Extensions.BackgroundTask.Telemetry;

namespace DCA.Extensions.BackgroundTask.PerfTest;
public static class Counter
{
    private static int s_inflight = 0;
    public static int InflightTasks => s_inflight;

    public static void Init()
    {
        var listener = new MeterListener();
        listener.InstrumentPublished = (instrument, listener) =>
        {
            Console.WriteLine($"{instrument.Name} {instrument}");
            if (instrument.Meter.Name == Metrics.MeterName)
            {
                listener.EnableMeasurementEvents(instrument);
            }
        };

        listener.SetMeasurementEventCallback<int>(OnMeasurementRecorded);
        listener.Start();
    }

    private static void OnMeasurementRecorded(Instrument instrument, int measurement, ReadOnlySpan<KeyValuePair<string, object?>> tags, object? state)
    {
        if (instrument.Name == Metrics.CounterInflightBackgroundTasks.Name)
        {
            Interlocked.Add(ref s_inflight, measurement);
        }
    }

}
