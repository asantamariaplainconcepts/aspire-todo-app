using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Xunit.Abstractions;

[assembly: TestFramework("AspireTest.Seedwork.Monitoring.OtelTestFramework", "AspireTest")]

namespace AspireTest.Seedwork.Monitoring;

public class OtelTestFramework : TracedTestFramework {
    public OtelTestFramework(IMessageSink messageSink) : base(messageSink) {
        traceProviderSetup = tpb => {
            tpb
                .ConfigureResource(resource => resource.AddService(ApiServiceFixture.ActivitySourceName))
                .AddSource(ApiServiceFixture.ActivitySourceName)
                .AddConsoleExporter()
                .AddOtlpExporter();
        };
    }
}
