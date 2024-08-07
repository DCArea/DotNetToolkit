﻿namespace DCA.Extensions.BackgroundTask;

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
    /// <param name="id">Task id</param>
    /// <param name="channel">Channel key</param>
    /// <param name="startNow">Should the task start immediately</param>
    /// <returns></returns>
    ValueTask DispatchAsync<TContext>(
        Func<TContext, ValueTask> taskDelegate,
        TContext taskContext,
        string? id = null,
        string? channel = null,
        bool startNow = true) where TContext : IBackgroundTaskContext;
}
