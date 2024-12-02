using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Notifications.Diagnostics;

public static class Instrumentation
{
    public const string ServiceName = "notifications";

    public static ActivitySource Source = new ActivitySource(ServiceName);
}
