using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;
namespace DCA.Extensions.BackgroundTask.Test;

public class HostedServiceTests
{
    [Fact]
    public async Task StopTwice()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddBackgroundTask();
        services.AddScoped<ServiceA>();
        var provider = services.BuildServiceProvider();
        var host = (BackgroundTaskHostedService) provider.GetRequiredService<IHostedService>();
        await host.StartAsync(default);
        await host.StopAsync(default);
        await host.StopAsync(default);
    }
}
