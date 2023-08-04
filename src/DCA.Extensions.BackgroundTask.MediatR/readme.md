MediatR extension for `DCA.Extensions.BackgroundTask`

## How to use it

```csharp
// define background task and it's handler base on MediatR
public record MyTask(string Id) : IRequest
{
    public class MyTaskHandler : IRequestHandler<MyTask>
    {
        public Task Handle(MyTask request, CancellationToken cancellationToken)
        {
			// do work
            return Task.CompletedTask;
        }
    }
}

```

```csharp
services.AddBackgroundTask();
// Register MediatR
services.AddMediatR(opt => opt.RegisterServicesFromAssemblyContaining<MyTask>());

// dispatch a task to run in background
var dispatcher = serviceProvider.GetRequiredService<IBackgroundTaskDispatcher>();
var task = new MyTask(Guid.NewGuid().ToString());
await dispatcher.DispatchAsync(task);
```
