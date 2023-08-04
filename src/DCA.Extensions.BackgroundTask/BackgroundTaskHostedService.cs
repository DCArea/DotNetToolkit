using System.Collections.ObjectModel;
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
    ReadOnlyCollection<BackgroundTaskChannel> channels) : IHostedService
{
    private readonly ILogger _logger = logger;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting channels...");
        await Task.WhenAll(channels.Select(x => x.StartAsync()));
        _logger.LogInformation("Started channels...");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping background channels...");
        await Task.WhenAny(channels.Select(x => x.StopAsync()));
        _logger.LogInformation("Stopped background channels...");
    }
}
