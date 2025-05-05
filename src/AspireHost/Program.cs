using aspire.AppHost.Integrations;
using static AspireHost.Constants;

var builder = DistributedApplication.CreateBuilder(args);

var password = builder.AddParameter("password", "password", secret: true);
var user = builder.AddParameter("username", "user", secret: true);

var sqlServer = builder.AddAzurePostgresFlexibleServer("sql-server")
    .WithPasswordAuthentication(user, password);

var sql = sqlServer.AddDatabase(Database);

var cache = builder.AddRedis("cache");

var mail = builder.AddMailDev("mail")
    .WithExternalHttpEndpoints();

builder.AddProject<Projects.DbService>("db-service")
    .WithHttpCommand("/reset-db", "Reset Database", commandOptions: new HttpCommandOptions
    {
        IconName = "DatabaseLightning",
    })
    .WithReference(sql)
    .WaitFor(sql);


var api = builder.AddProject<Projects.Api>(Api)
    .WithReference(sql)
    .WithReference(cache)
    .WithReference(mail)
    .WithExternalHttpEndpoints()
    .WaitFor(sql);

builder.AddViteApp("vue", "../spa")
    .WithNpmPackageInstallation()
    .WithReference(api)
    .WithOtlpExporter()
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

if (builder.ExecutionContext.IsPublishMode)
{
}

else
{
    if (builder.Environment.EnvironmentName.Contains("Test"))
    {
        api.WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:4317");
        sqlServer.RunAsContainer(c => c.WithLifetime(ContainerLifetime.Session));
    }
    else
    {
        sqlServer
            .RunAsContainer(c =>
                c.WithLifetime(ContainerLifetime.Persistent)
                    .WithPgWeb());

        cache.WithLifetime(ContainerLifetime.Persistent)
            .WithRedisInsight();

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
}

builder.Build().Run();