using aspire.AppHost.Integrations;
using AspireHost.Extensions;
using static AspireHost.Constants;

var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddSqlServer("sql");

var sql = sqlServer.AddDatabase(Database);
    

var cache = builder.AddRedis("cache");

var queue = builder.AddRabbitMQ("queue");

var mail = builder.AddMailDev("mail");

var dbService = builder.AddProject<Projects.DbService>("db-service")
    .WithHttpHealthCheck("/health")
    .WithHttpCommand("/reset-db", "Reset Database", iconName: "DatabaseLightning")
    .WithReference(sql, "SqlServer")
    .WaitFor(sql);

var api = builder.AddProject<Projects.Api>(Api)
    .WithReference(sql, "SqlServer")
    .WithReference(cache)
    .WithReference(queue)
    .WithReference(mail)
    .WithExternalHttpEndpoints();

var web = builder.AddProject<Projects.Web>("blazor")
    .WithExternalHttpEndpoints()
    .WithReference(api)
    .WithReplicas(2)
    .WaitFor(api)
    .WaitFor(dbService);

var spa = builder.AddViteApp("vue", "../spa")
    .WithNpmPackageInstallation()
    .WithReference(api)
    .WaitFor(dbService);

if (builder.Environment.EnvironmentName.Contains("Test"))
{
    api.WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:4317");
}
else
{
    sqlServer.WithLifetime(ContainerLifetime.Persistent);
       
    cache.WithRedisCommander();

    queue.WithManagementPlugin();
      
    var storage = builder.AddAzureStorage("storage")
        .RunAsEmulator()
        .AddBlobs("blobs");
    
    var seq = builder.AddSeq("seq");

    api.WithReference(seq)
        .WithReference(storage);
    
    var postgres = builder.AddPostgres("testing-postgres")
        .WithPgAdmin()
        .AddDatabase("testing-postgres-db");
    
    // var ollama = builder.AddOllama("ollama")
    //     .AddModel("phi3.5");
}

builder.Build().Run();