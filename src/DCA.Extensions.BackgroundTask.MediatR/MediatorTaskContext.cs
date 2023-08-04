using MediatR;

namespace DCA.Extensions.BackgroundTask;

internal record MediatorTaskContext<TTask>(
    IServiceProvider ServiceProvider,
    TTask Task
) : IBackgroundTaskContext
where TTask: IRequest;
