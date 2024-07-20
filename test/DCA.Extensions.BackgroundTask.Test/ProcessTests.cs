using System.Collections.Frozen;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;
namespace DCA.Extensions.BackgroundTask.Test;

public class ReaderTests
{
    [Fact]
    public async Task Should_RecordCheckpoints()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddBackgroundTask();
        var provider = services.BuildServiceProvider();

        var channels = provider.GetRequiredService<FrozenDictionary<string, BackgroundTaskChannel>>();
        var defaultChannel = channels[Constants.DefaultChannelKey];
        var host = (BackgroundTaskHostedService)provider.GetRequiredService<IHostedService>();
        await host.StartAsync(default);

        var dispatcher = provider.GetRequiredService<IBackgroundTaskDispatcher>();
        for (int i = 0; i < 20; i++)
        {
            await dispatcher.DispatchAsync(ctx => new ValueTask(Task.Delay(200)), new MyTaskContext(i));
        }

        SpinWait.SpinUntil(() =>
        {
            var checkpoint = defaultChannel.Checkpoints.Cast<BackgroundTask<MyTaskContext>>().FirstOrDefault();
            return checkpoint?.Context.Id == 19;
        }, 5000).Should().BeTrue();

        await host.StopAsync(default);
        var checkpoint = defaultChannel.Checkpoints.Cast<BackgroundTask<MyTaskContext>>().Single();
        checkpoint.Context.Id.Should().Be(19);
    }

}

public record MyTaskContext(int Id) : IBackgroundTaskContext;
