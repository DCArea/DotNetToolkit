using Microsoft.Extensions.Logging;

namespace DCA.Extensions.BackgroundTask.Telemetry;

internal static partial class Logs
{
    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "[{channel}] Dispatched a task to execute in background"
    )]
    public static partial void BackgroundTaskDispatched(ILogger logger, string channel);

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
}
