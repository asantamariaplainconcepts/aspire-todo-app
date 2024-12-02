using aspire.AppHost.Integrations;
using AspireHost.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using static AspireHost.Constants;

var builder = DistributedApplication.CreateBuilder(args);

var server = builder.AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent)
    .PublishAsAzureSqlDatabase()
    .WithDataVolume("db-dotnet-malaga");

var sql = server.AddDatabase(Database);

var cache = builder.AddRedis("cache")
    .WithRedisCommander();

var queue = builder.AddRabbitMQ("queue")
    .WithManagementPlugin();

var storage = builder.AddAzureStorage("storage")
    .RunAsEmulator(x => x.WithLifetime(ContainerLifetime.Persistent))
    .AddBlobs("blobs");

var mail = builder.AddMailDev("mail");

var seq = builder.AddSeq("seq");

var dbService = builder.AddProject<Projects.DbService>("db-service")
    .WithHttpHealthCheck("/health")
    .WithHttpCommand("/reset-db", "Reset Database", iconName: "DatabaseLightning")
    .WithReference(sql, "SqlServer")
    .WaitFor(sql);

var api = builder.AddProject<Projects.Api>(Api)
    .WithReference(sql, "SqlServer")
    .WithReference(cache)
    .WithReference(queue)
    .WithReference(seq)
    .WithReference(mail)
    .WithReference(storage)
    .WithExternalHttpEndpoints()
    .WaitFor(dbService);

var web = builder.AddProject<Projects.Web>("blazor")
    .WithExternalHttpEndpoints()
    .WithReplicas(2)
    .WithReference(api)
    .WaitFor(api)
    .WaitFor(dbService);

var spa = builder.AddViteApp("vue", "../spa")
    .WithNpmPackageInstallation()
    .WithReference(api)
    .WaitFor(dbService);

builder.Eventing.Subscribe<ResourceReadyEvent>(
    cache.Resource,
    static (@event, _) =>
    {
        var logger = @event.Services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Cache ResourceReadyEvent");

        return Task.CompletedTask;
    });

if (builder.Environment.EnvironmentName.Contains("Test"))
{
    api.WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:4317");
}
else
{
    var postgres = builder.AddPostgres("testing-postgres")
        .WithPgWeb()
        .AddDatabase("testing-postgres-db");


    // var ollama = builder.AddOllama("ollama")
    //     .AddModel("phi3.5");
}

builder.Build().Run();