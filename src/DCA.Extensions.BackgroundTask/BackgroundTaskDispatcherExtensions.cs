using Microsoft.Extensions.DependencyInjection;

namespace DCA.Extensions.BackgroundTask;

internal record DelegateBackgroundTaskContext<TDependency>(
    IServiceProvider ServiceProvider,
    Func<TDependency, ValueTask> TaskDelegate
) : IBackgroundTaskContext;

public static class BackgroundTaskDispatcherExtensions
{
    /// <summary>
    /// Dispatch a background task
    /// </summary>
    /// <typeparam name="TDependency">Dependency type</typeparam>
    /// <param name="dispatcher"></param>
    /// <param name="taskDelegate">Task delegate</param>
    /// <param name="id">Task id</param>
    /// <param name="channel">Channel name</param>
    /// <param name="startNow">Should this tast start now</param>
    /// <returns></returns>
    public static ValueTask DispatchAsync<TDependency>(
        this IBackgroundTaskDispatcher dispatcher,
        Func<TDependency, ValueTask> taskDelegate,
        string? id = null,
        string? channel = null,
        bool startNow = true) where TDependency : notnull
    {
        var context = new DelegateBackgroundTaskContext<TDependency>(
            dispatcher.ServiceProvider,
            taskDelegate
        );
        return dispatcher.DispatchAsync(Execute, context, id, channel, startNow);

        static ValueTask Execute(DelegateBackgroundTaskContext<TDependency> context)
        {
            using var scope = context.ServiceProvider.CreateScope();
            var dependency = scope.ServiceProvider.GetRequiredService<TDependency>();
            return context.TaskDelegate(dependency);
        }
    }
}
