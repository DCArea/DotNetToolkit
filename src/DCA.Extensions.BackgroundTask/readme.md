A simple, in-memory background task dispatcher and processor

## How to use it

```csharp
// Register background task components
services.AddBackgroundTask();
// Register background dependency
services.AddScoped<ServiceA>();

// dispatch a task to run in background
var dispatcher = serviceProvider.GetRequiredService<IBackgroundTaskDispatcher>();
await dispatcher.DispatchAsync<ServiceA>(async dep =>
{
	await dep.DoWork();
});

```

## Configuration

```csharp
services.AddBackgroundTask(opt =>
{
	opt.Channels.Add(new BackgroundTaskChannelOptions{
		Key = "MyChannelKey",
		Capacity = 1024, // Max number of inflight tasks in the channel, A negetive number means unlimited
		WorkerCount = 1 // Number of workers for the channel, it should be 1 if all tasks in the channel are started immediately when dispatching
	});
});
```