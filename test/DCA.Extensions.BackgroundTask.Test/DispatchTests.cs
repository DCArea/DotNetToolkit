using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;
namespace DCA.Extensions.BackgroundTask.Test;

public class DispatchTests
{
    [Fact]
    public async Task DispatchDelegate()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddBackgroundTask();
        services.AddScoped<ServiceA>();
        var provider = services.BuildServiceProvider();

        var host = (BackgroundTaskHostedService) provider.GetRequiredService<IHostedService>();
        await host.StartAsync(default);

        var dispatcher = provider.GetRequiredService<IBackgroundTaskDispatcher>();
        await dispatcher.DispatchAsync<ServiceA>(async dep => await dep.DoWork());
        await host.StopAsync(default);
    }
}

public class ServiceA
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
    public async Task DoWork()
    {
        // await Task.Delay(10*1000);
        await Task.Yield();
    }
}