using aspire.AppHost.Integrations;
using AspireHost.Extensions;
using static AspireHost.Constants;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddDockerComposePublisher();

var sqlServer = builder.AddAzureSqlServer("sql-server");

var sql = sqlServer.AddDatabase(Database);

var cache = builder.AddAzureRedis("cache");

var mail = builder.AddMailDev("mail");

var dbService = builder.AddProject<Projects.DbService>("db-service")
    .WithHttpCommand("/reset-db", "Reset Database", iconName: "DatabaseLightning")
    .WithReference(sql, "SqlServer")
    .WaitFor(sql)
    .WithExplicitStart();

var api = builder.AddProject<Projects.Api>(Api)
    .WithReference(sql, "SqlServer")
    .WithReference(cache)
    .WithReference(mail)
    .WithExternalHttpEndpoints()
    .WaitFor(sql)
    .PublishAsDockerFile(cfg => cfg
    .WithDockerfile("src/AspireHost/Projects/Api/Dockerfile"));

var spa = builder.AddViteApp("vue", "../spa")
    .WithNpmPackageInstallation()
    .WithReference(api)
    .WithOtlpExporter();

if (builder.Environment.EnvironmentName.Contains("Test"))
{
    api.WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:4317");

    sqlServer.RunAsContainer(c => c.WithLifetime(ContainerLifetime.Session));
}
else
{
    sqlServer
        .RunAsContainer(c => c.WithLifetime(ContainerLifetime.Persistent));

    cache
        .RunAsContainer(c => c.WithLifetime(ContainerLifetime.Persistent)
            .WithRedisCommander());
    
    var seq = builder.AddSeq("seq");
    
    api.WithReference(seq)
        .WithUrls(ctx =>
        {
            ctx.Urls.Add(new ResourceUrlAnnotation
            {
                Url = $"{api.GetEndpoint("https").Url}/swagger",
                DisplayText = "Swagger UI",
            });
            ctx.Urls.Add(new ResourceUrlAnnotation
            {
                Url = $"{api.GetEndpoint("https").Url}/cap",
                DisplayText = "Cap UI",
            });
        });
}

builder.Build().Run();