namespace AspireHost.Extensions;

internal static class AddTodoMigration
{
    public static IResourceBuilder<ExecutableResource>? AddTodoDbMigration(this IDistributedApplicationBuilder builder,
        IResourceBuilder<PostgresDatabaseResource> sql)
    {
        IResourceBuilder<ExecutableResource>? migrateOperation = default;

        if (builder.ExecutionContext.IsRunMode)
        {
            var projectDirectory = Path.GetDirectoryName(new Projects.Todos().ProjectPath)!;

            var connectionString = new ConnectionStringReference(sql.Resource, optional: false);

            return builder.AddExecutable("todo-db-migration", "dotnet", projectDirectory, "ef", "database", "update",
                    "--no-build", "--connection", connectionString, "--context", "TodoDbContext")
                .WaitFor(sql);
        }

        return migrateOperation;
    }
}