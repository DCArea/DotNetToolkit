using System.Collections.Frozen;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace DCA.Extensions.BackgroundTask;

public sealed class BackgroundTaskDispatcher(
    FrozenDictionary<string, BackgroundTaskChannel> channels,
    //ILogger<BackgroundTaskDispatcher> logger,
    ILoggerFactory loggerFactory,
    IServiceProvider serviceProvider)
    : IBackgroundTaskDispatcher
{
    private readonly ILogger logger = loggerFactory.CreateLogger<IBackgroundTask>();
    public IServiceProvider ServiceProvider { get; } = serviceProvider;

    public ValueTask DispatchAsync<TContext>(
        Func<TContext, ValueTask> taskDelegate,
        TContext taskContext,
        string? id = null,
        string? channel = null,
        bool startNow = true) where TContext : IBackgroundTaskContext
    {
        var task = new BackgroundTask<TContext>(
            id ?? Guid.NewGuid().ToString(),
            taskDelegate,
            taskContext,
            logger,
            Activity.Current?.Context ?? default
        );

        channel ??= Constants.DefaultChannelKey;
        return channels[channel].DispatchAsync(task, startNow);
    }
}
