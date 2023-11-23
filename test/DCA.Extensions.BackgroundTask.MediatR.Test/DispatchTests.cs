using System.Collections.Concurrent;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace DCA.Extensions.BackgroundTask.MediatR.Test;

public class DispatchTests
{
    [Fact]
    public async Task Dispatch()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddBackgroundTask();
        services.AddMediatR(opt => opt.RegisterServicesFromAssemblyContaining<MyTask>());
        var provider = services.BuildServiceProvider();
        var host = (BackgroundTaskHostedService)provider.GetRequiredService<IHostedService>();
        await host.StartAsync(default);

        var dispatcher = provider.GetRequiredService<IBackgroundTaskDispatcher>();
        var task = new MyTask(Guid.NewGuid().ToString());
        await dispatcher.DispatchAsync(task);
        await host.StopAsync(default);

        MyTask.MyTaskHandler.ProcessedTaskIds.Should().Contain(task.Id);
    }
}

public record MyTask(string Id) : IRequest
{
    public class MyTaskHandler : IRequestHandler<MyTask>
    {
        private static readonly ConcurrentBag<string> _processedTaskIds = [];

        public static ConcurrentBag<string> ProcessedTaskIds => _processedTaskIds;

        public Task Handle(MyTask request, CancellationToken cancellationToken)
        {
            ProcessedTaskIds.Add(request.Id);
            return Task.CompletedTask;
        }
    }
}
