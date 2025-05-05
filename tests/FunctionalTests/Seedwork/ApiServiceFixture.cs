﻿using System.Diagnostics;
using Api;
using DbSeeder;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Respawn;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;
using Todos.Infrastructure.Persistence;

namespace FunctionalTests.Seedwork;

public sealed class ApiServiceFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private Respawner? _respawner;

    private readonly  PostgreSqlContainer _sqlContainer = new PostgreSqlBuilder().Build();
    private readonly RedisContainer _redisContainer = new RedisBuilder().Build();
    private readonly RabbitMqContainer _rabbitMqContainer = new Testcontainers.RabbitMq.RabbitMqBuilder().Build();
    private readonly IContainer _mailDevContainer = new ContainerBuilder()
        .WithName(Guid.NewGuid().ToString("D"))
        .WithImage("maildev/maildev:2.0.2")
        .WithPortBinding(1025, true)
        .Build();
    
    public static string ActivitySourceName => "FunctionalTests";

    public static ActivitySource ActivitySource { get; } = new(ActivitySourceName);
    public HttpClient Client { get; private set; } = new();

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(config =>
        {
            config.AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string?>("ConnectionStrings:TodoAppDb", _sqlContainer.GetConnectionString()),
                new KeyValuePair<string, string?>("ConnectionStrings:redis", _redisContainer.GetConnectionString()),
                new KeyValuePair<string, string?>("ConnectionStrings:mail", $"smtp://{_mailDevContainer.Hostname}:{_mailDevContainer.GetMappedPublicPort(1025)}"),
                new KeyValuePair<string, string?>("ConnectionStrings:queue", _rabbitMqContainer.GetConnectionString())
            });
        });

        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseTestServer();
    }

    public async Task InitializeAsync()
    {
        using var activityScope = ActivitySource.StartActivity("Tear_Up");

        await _sqlContainer.StartAsync();
        await _redisContainer.StartAsync();
        await _mailDevContainer.StartAsync();
        await _rabbitMqContainer.StartAsync();

        // var dbContext = new TodoDbContext(new DbContextOptionsBuilder<TodoDbContext>()
        //     .UseSqlServer(_sqlContainer.GetConnectionString())
        //     .Options);
        //
        // await dbContext.Database.EnsureCreatedAsync();
        // await dbContext.Database.MigrateAsync();

        var migrator = new DbMigrate(NullLogger.Instance);
        var result = migrator.Migrate(_sqlContainer.GetConnectionString());
        if (!result.Successful)
            throw new Exception("Migration failed");

        _respawner = await Respawner.CreateAsync(_sqlContainer.GetConnectionString());

        Client = Server.CreateClient();
    }

    public new async Task DisposeAsync()
    {
        using var activityScope = ActivitySource.StartActivity("Tear_Down");
        await _sqlContainer.StopAsync();
        await _redisContainer.StopAsync();
        await _mailDevContainer.StopAsync();
        await _rabbitMqContainer.StopAsync();
    }

    public async Task Reset()
    {
        if (_respawner != null)
            await _respawner.ResetAsync(_sqlContainer.GetConnectionString()!);
    }

    public async Task ExecuteDbContextAsync(Func<TodoDbContext, Task> function)
    {
        var factory = Services.GetRequiredService<IServiceScopeFactory>();

        using var scope = factory.CreateScope();
        var context = scope.ServiceProvider
            .GetRequiredService<TodoDbContext>();

        await function(context);
    }
}