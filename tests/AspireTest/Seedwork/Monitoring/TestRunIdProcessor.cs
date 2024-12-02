using System.Diagnostics;
using OpenTelemetry;

namespace AspireTest.Seedwork.Monitoring;

internal class TestRunIdProcessor : BaseProcessor<Activity>
{
    private static readonly Guid _testRunId = Guid.NewGuid();

    public override void OnStart(Activity activity)
    {
        activity.SetTag("test.run_id", _testRunId.ToString());
    }
}
