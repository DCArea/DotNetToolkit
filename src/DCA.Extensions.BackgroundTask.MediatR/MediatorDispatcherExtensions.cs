using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace DCA.Extensions.BackgroundTask;

public static class MediatorDispatcherExtensions
{

    /// <summary>
    /// Dispatch a background task
    /// </summary>
    /// <typeparam name="TTask">Task payload type</typeparam>
    /// <param name="dispatcher"></param>
    /// <param name="task">Task payload</param>
    /// <param name="channel">Channel name</param>
    /// <param name="startNow">Should this tast start now</param>
    /// <returns></returns>
    public static ValueTask DispatchAsync<TTask>(
        this IBackgroundTaskDispatcher dispatcher,
        TTask task,
        string? channel = null,
        bool startNow = true) where TTask : IRequest
    {
        var context = new MediatorTaskContext<TTask>(
            dispatcher.ServiceProvider,
            task
        );
        return dispatcher.DispatchAsync(Execute, context, channel, startNow);

        static ValueTask Execute(MediatorTaskContext<TTask> context)
        {
            var mediator = context.ServiceProvider.GetRequiredService<IMediator>();
            return new ValueTask(mediator.Send(context.Task));
        }
    }
}
