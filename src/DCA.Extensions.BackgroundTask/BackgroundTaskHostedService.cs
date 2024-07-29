using System.Collections.Frozen;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DCA.Extensions.BackgroundTask;

/// <summary>
/// A hosted service to manage task channels' lifetime
/// </summary>
/// <param name="logger"></param>
/// <param name="channels"></param>
public class BackgroundTaskHostedService(
    ILogger<BackgroundTaskHostedService> logger,
    FrozenDictionary<string, BackgroundTaskChannel> channels) : IHostedService
{
    private readonly ILogger _logger = logger;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting channels...");
        await Task.WhenAll(channels.Values.Select(x => x.StartAsync()));
        _logger.LogInformation("Started channels...");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping background channels...");
        await Task.WhenAll(channels.Values.Select(x => x.StopAsync()));
        // await Task.WhenAny(channels.Values.Select(x => x.StopAsync()));
        _logger.LogInformation("Stopped background channels...");
    }
}
