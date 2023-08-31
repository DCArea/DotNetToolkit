using System.Diagnostics;
using DCA.Extensions.BackgroundTask.Telemetry;
using Microsoft.Extensions.Logging;

namespace DCA.Extensions.BackgroundTask;

public interface IBackgroundTaskContext;

public interface IBackgroundTask : IThreadPoolWorkItem
{
    bool Started { get; }
    void Start();
    ValueTask WaitToCompleteAsync();
}

public sealed class BackgroundTask<TContext>(
    string? id,
    Func<TContext, ValueTask> taskDelegate,
    TContext context,
    ILogger logger,
    ActivityContext activityContext
    ) : IBackgroundTask, IThreadPoolWorkItem
    where TContext : IBackgroundTaskContext
{
    private readonly WorkItemWaiter _waiter = new();
    public bool Started => _started == 1;
    private int _started = 0;
    public string? Id => id;
    public TContext Context => context;

    void IThreadPoolWorkItem.Execute() => Start();

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2012:Use ValueTasks correctly", Justification = "<Pending>")]
    public void Start()
    {
        if (Interlocked.CompareExchange(ref _started, 1, 0) == 0)
        {
            _ = ExecuteAsync();
        }
        else
        {
            return;
        }
    }

    public ValueTask WaitToCompleteAsync()
        => _waiter.Task;

    internal async ValueTask ExecuteAsync()
    {
        Activity? activity = null;
        if (activityContext != default)
        {
            activity = Tracing.ActivitySource.StartActivity(
                "Execute Background Task",
                ActivityKind.Internal,
                activityContext);
        }
        if (id != null)
        {
            activity?.SetTag("task.id", id);
        }
        try
        {
            await taskDelegate(context).ConfigureAwait(false);
            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error);
            logger.LogError(ex, "Error on executing task {TaskId}", id);
        }
        finally
        {
            activity?.Dispose();
            _waiter.SetResult();
        }
    }
}
