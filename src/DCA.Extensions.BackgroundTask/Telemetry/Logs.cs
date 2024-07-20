using Microsoft.Extensions.Logging;

namespace DCA.Extensions.BackgroundTask.Telemetry;

internal static partial class Logs
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "[{channel}] Dispatched a task[{id}] to execute in background"
    )]
    public static partial void BackgroundTaskDispatched(ILogger logger, string channel, string? id);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "[{channel}] Reader stopped"
    )]
    public static partial void ReaderStopped(ILogger logger, string channel);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "[{channel}] Reader error"
    )]
    public static partial void ReaderError(ILogger logger, Exception ex, string channel);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "[{channel}] Task[{id}] not started, starting"
    )]
    public static partial void TaskStarting(ILogger logger, string channel, string? id);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "[{channel}] Waiting task[{id}] to complete"
    )]
    public static partial void TaskWaitingToComplete(ILogger logger, string channel, string? id);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "[{channel}] No task in channel, waiting for next"
    )]
    public static partial void WaitingNextTask(ILogger logger, string channel);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Task[{id}] executing"
    )]
    public static partial void BackgroundTaskExecuting(ILogger logger, string? id);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Task[{id}] executed"
    )]
    public static partial void BackgroundTaskExecuted(ILogger logger, string? id);
}
