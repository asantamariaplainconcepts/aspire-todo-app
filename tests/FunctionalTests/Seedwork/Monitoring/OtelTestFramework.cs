using E2ETests.Seedwork.Monitoring;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Xunit.Abstractions;

[assembly: TestFramework("FunctionalTests.Seedwork.Monitoring", "E2ETests")]

namespace FunctionalTests.Seedwork.Monitoring;

public class OtelTestFramework : TracedTestFramework {
    public OtelTestFramework(IMessageSink messageSink) : base(messageSink) {
        traceProviderSetup = tpb => {
            tpb
                .ConfigureResource(resource => resource.AddService("E2E-Tests"))
                .AddSource(ApiServiceFixture.ActivitySourceName)
                .AddConsoleExporter()
                .AddOtlpExporter();
        };
    }
}
