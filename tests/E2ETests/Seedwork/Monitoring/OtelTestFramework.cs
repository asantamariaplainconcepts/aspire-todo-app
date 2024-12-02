using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Xunit.Abstractions;

[assembly: TestFramework("E2ETests.Seedwork.Monitoring.OtelTestFramework", "E2ETests")]

namespace E2ETests.Seedwork.Monitoring;

public class OtelTestFramework : TracedTestFramework {
    public OtelTestFramework(IMessageSink messageSink) : base(messageSink) {
        traceProviderSetup = tpb => {
            tpb
                .ConfigureResource(resource => resource.AddService("E2E-Tests"))
                .AddSource(AppHostFixture.ActivitySourceName)
                .AddConsoleExporter()
                .AddOtlpExporter();
        };
    }
}
