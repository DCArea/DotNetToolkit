using System.Collections.ObjectModel;
using DCA.Extensions.BackgroundTask;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class BackgroundTaskServiceCollectionExtensions
{
    /// <summary>
    /// Register background task services
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddBackgroundTask(this IServiceCollection services)
        => services.AddBackgroundTask(null);

    /// <summary>
    /// Register background task services
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configureOptions">Configure</param>
    /// <returns></returns>
    public static IServiceCollection AddBackgroundTask(
        this IServiceCollection services,
        Action<BackgroundTaskOptions>? configureOptions)
    {
        var ob = services.AddOptions<BackgroundTaskOptions>();
        if (configureOptions != null)
        {
            ob.Configure(configureOptions);
        }
        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<BackgroundTaskOptions>>().Value;
            return options.Channels
                .Select(opt => ActivatorUtilities.CreateInstance<BackgroundTaskChannel>(sp, opt))
                .ToList()
                .AsReadOnly();
        });
        services.AddSingleton<IBackgroundTaskDispatcher, BackgroundTaskDispatcher>();
        services.AddHostedService<BackgroundTaskHostedService>();
        return services;
    }
}
