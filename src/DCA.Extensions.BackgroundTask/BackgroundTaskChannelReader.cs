using System.Diagnostics;
using System.Threading.Channels;
using DCA.Extensions.BackgroundTask.Telemetry;
using Microsoft.Extensions.Logging;

namespace DCA.Extensions.BackgroundTask;

internal sealed class BackgroundTaskChannelReader(
    string key,
    ILogger logger,
    ChannelReader<IBackgroundTask> channelReader,
    CancellationToken stopToken)
{
    private Task _readLoop = default!;
    public IBackgroundTask? Checkpoint;

    public Task StartAsync()
    {
        _readLoop = Task.Run(ReadLoop);
        return Task.CompletedTask;
    }

    public async Task StopAsync()
    {
        await _readLoop;
    }

    private async Task ReadLoop()
    {
        try
        {
            while (true)
            {
                if (channelReader.TryRead(out var task))
                {
                    var tagList = new TagList
                    {
                        { "channel", key }
                    };
                    Metrics.CounterInflightBackgroundTasks.Add(-1, tagList);
                    Metrics.CounterProcessedTasks.Add(1);
                    if (!task.Started)
                    {
                        task.Start();
                    }
                    var vt = task.WaitToCompleteAsync();
                    if (!vt.IsCompletedSuccessfully)
                    {
                        await vt.ConfigureAwait(false);
                    }
                    Checkpoint = task;
                }
                else
                {
                    if (stopToken.IsCancellationRequested)
                    {
                        Logs.ReaderStopped(logger, key);
                        return;
                    }
                    else
                    {
                        await channelReader.WaitToReadAsync(stopToken).ConfigureAwait(false);
                    }
                }
            }
        }
        catch (OperationCanceledException ex) when (ex.CancellationToken == stopToken)
        {
            Logs.ReaderStopped(logger, key);
        }
        catch (Exception ex)
        {
            Logs.ReaderError(logger, ex, key);
        }
    }
}
