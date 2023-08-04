using System.Diagnostics;

namespace DCA.Extensions.BackgroundTask.Telemetry;

internal static class Tracing
{
    public static readonly ActivitySource ActivitySource = new("DCA.DotNetToolkit");
}
