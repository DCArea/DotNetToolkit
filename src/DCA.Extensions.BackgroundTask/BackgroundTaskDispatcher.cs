using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace DCA.Extensions.BackgroundTask;

/// <summary>
/// Background task dispatcher
/// </summary>
public interface IBackgroundTaskDispatcher
{
    /// <summary>
    /// An <see cref="IServiceProvider"/> instance, used to create a scope for each task execution
    /// </summary>
    IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Dispatch a background task
    /// </summary>
    /// <typeparam name="TContext">Type of task context</typeparam>
    /// <param name="taskDelegate">Task delegate</param>
    /// <param name="taskContext">Task context</param>
    /// <param name="channel">Channel key</param>
    /// <param name="startNow">Should the task start immediately</param>
    /// <returns></returns>
    ValueTask DispatchAsync<TContext>(
        Func<TContext, ValueTask> taskDelegate,
        TContext taskContext,
        string? channel = null,
        bool startNow = true) where TContext : IBackgroundTaskContext;
}

public sealed class BackgroundTaskDispatcher(
    ReadOnlyCollection<BackgroundTaskChannel> channels,
    ILogger<BackgroundTaskDispatcher> logger,
    IServiceProvider serviceProvider)
    : IBackgroundTaskDispatcher
{
    private readonly ReadOnlyDictionary<string, BackgroundTaskChannel> _channels
        = new(channels.ToDictionary(x => x.Key, x => x));

    public IServiceProvider ServiceProvider { get; } = serviceProvider;

    public ValueTask DispatchAsync<TContext>(
        Func<TContext, ValueTask> taskDelegate,
        TContext taskContext,
        string? channel = null,
        bool startNow = true) where TContext : IBackgroundTaskContext
    {
        var task = new BackgroundTask<TContext>(
            null,
            taskDelegate,
            taskContext,
            logger,
            Activity.Current?.Context ?? default
        );

        channel ??= Constants.DefaultChannelKey;
        return _channels[channel].DispatchAsync(task, startNow);
    }
}
