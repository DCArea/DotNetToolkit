namespace DCA.Extensions.BackgroundTask;

public class BackgroundTaskOptions
{
    /// <summary>
    /// Configure channels
    /// </summary>
    public List<BackgroundTaskChannelOptions> Channels { get; set; } =
    [
        new (){ Key = Constants.DefaultChannelKey}
    ];
}


public class BackgroundTaskChannelOptions
{
    /// <summary>
    /// The key of the channel
    /// </summary>
    public required string Key { get; init; }

    /// <summary>
    /// Max number of inflight tasks in the channel, A negetive number means unlimited
    /// </summary>
    public int Capacity { get; set; } = 32;

    /// <summary>
    /// Number of workers for the channel, it should be 1 if all tasks in the channel are started immediately when dispatching
    /// </summary>
    public int WorkerCount { get; set; } = 1;
}
