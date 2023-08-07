using System.Diagnostics;
using System.Threading.Channels;
using DCA.Extensions.BackgroundTask.Telemetry;
using Microsoft.Extensions.Logging;

namespace DCA.Extensions.BackgroundTask;

/// <summary>
/// Background task channel
/// </summary>
public sealed class BackgroundTaskChannel
{
    private readonly Channel<IBackgroundTask> _channel;
    private readonly List<BackgroundTaskChannelReader> _readers = new();
    private readonly CancellationTokenSource _stopTokenSource = new();
    private readonly ILogger<BackgroundTaskChannel> _logger;
    private readonly BackgroundTaskChannelOptions _options;

    public string Key { get; }

    public BackgroundTaskChannel(ILogger<BackgroundTaskChannel> logger, BackgroundTaskChannelOptions options)
    {
        _logger = logger;
        _options = options;
        Key = options.Key;
        var singleReader = options.WorkerCount == 1;
        if (options.Capacity <= 0)
        {
            var channelOptions = new UnboundedChannelOptions()
            {
                SingleWriter = false,
                SingleReader = singleReader
            };
            _channel = Channel.CreateUnbounded<IBackgroundTask>(channelOptions);
        }
        else
        {
            var channelOptions = new BoundedChannelOptions(options.Capacity)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleWriter = false,
                SingleReader = singleReader
            };
            _channel = Channel.CreateBounded<IBackgroundTask>(channelOptions);
        }
    }

    /// <summary>
    /// Start the channel
    /// </summary>
    /// <returns></returns>
    public Task StartAsync()
    {
        for (int i = 0; i < _options.WorkerCount; i++)
        {
            _readers.Add(new BackgroundTaskChannelReader(Key,
                _logger,
                _channel.Reader,
                _stopTokenSource.Token));
        }
        return Task.WhenAll(_readers.Select(r => r.StartAsync()));
    }

    /// <summary>
    /// Stop the channel
    /// </summary>
    /// <returns></returns>
    public Task StopAsync()
    {
        _channel.Writer.TryComplete();
        _stopTokenSource.Cancel();
        return Task.WhenAll(_readers.Select(r => r.StopAsync()));
    }

    /// <summary>
    /// Dispatch a background task
    /// </summary>
    /// <param name="task">Task to run in background</param>
    /// <param name="startNow">Should start immediately</param>
    /// <returns></returns>
    public async ValueTask DispatchAsync(IBackgroundTask task, bool startNow)
    {
        if (!_channel.Writer.TryWrite(task))
        {
            await _channel.Writer.WriteAsync(task);
        }

        var tagList = new TagList
        {
            { "channel", Key }
        };
        Metrics.CounterInflightBackgroundTasks.Add(1, tagList);
        Metrics.CounterDispatchedTasks.Add(1);
        Logs.BackgroundTaskDispatched(_logger, Key);

        if (startNow)
        {
            ThreadPool.UnsafeQueueUserWorkItem(task, false);
        }
    }
}
