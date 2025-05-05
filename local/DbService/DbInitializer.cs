using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Todos.Domain;
using Todos.Infrastructure.Persistence;

namespace DbService;

internal class DbInitializer(IServiceProvider serviceProvider, ILogger<DbInitializer> logger)
    : BackgroundService
{
    public const string ActivitySourceName = "Migrations";

    private readonly ActivitySource _activitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TodoDbContext>();

        using var activity = _activitySource.StartActivity("Initializing database", ActivityKind.Client);
        await InitializeDatabaseAsync(dbContext, cancellationToken);
    }

    public async Task InitializeDatabaseAsync(TodoDbContext dbContext, CancellationToken cancellationToken = default)
    {
        var sw = Stopwatch.StartNew();

        var strategy = dbContext.Database.CreateExecutionStrategy();
        
        await strategy.ExecuteAsync(dbContext.Database.MigrateAsync, cancellationToken);

        await SeedAsync(dbContext, cancellationToken);

        logger.LogInformation("Database initialization completed after {ElapsedMilliseconds}ms",
            sw.ElapsedMilliseconds);
    }

    private async Task SeedAsync(TodoDbContext dbContext, CancellationToken cancellationToken)
    {
        logger.LogInformation("Seeding database");

        if (!await dbContext.Todos.AnyAsync(cancellationToken))
        {
            dbContext.Todos.AddRange(
                new Todo("Ines"),
                new Todo("Luisa"),
                new Todo("Martina")
            );

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}