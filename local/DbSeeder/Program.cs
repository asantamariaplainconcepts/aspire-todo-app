﻿using DbSeeder;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Todos.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("TodoAppDb");

if(connectionString == null)
{
    throw new Exception("Connection string not found");
}

builder.Services.AddSingleton<DbMigrate>();

var app = builder.Build();

var migrator = app.Services.GetRequiredService<DbMigrate>();

migrator.Migrate(connectionString);

