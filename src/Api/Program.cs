using Hellang.Middleware.ProblemDetails;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BuildingBlocks.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Notifications;
using Todos;
using Todos.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.AddApiServices();

builder.AddSeqEndpoint("seq");

builder.Services.AddSignalR();

AddCustomMasstransit(builder);

TodosModule.Install(builder);
NotificationsModule.Install(builder);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseProblemDetails()
    .UseHttpsRedirection()
    .UseRouting();

NotificationsModule.Map(app);

app.MapEndpoints();

app.Run();

void AddCustomMasstransit(WebApplicationBuilder webApplicationBuilder)
{
    var host = builder.Configuration.GetConnectionString("queue");

    webApplicationBuilder.Services.AddMassTransit(busConfigurator =>
    {
        busConfigurator.SetKebabCaseEndpointNameFormatter();

        busConfigurator.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host(host);
            cfg.ConfigureEndpoints(context);
        });

        busConfigurator.AddEntityFrameworkOutbox<TodoDbContext>(options =>
        {
            options.UseSqlServer();
            options.UseBusOutbox();
            options.DisableInboxCleanupService();
        });
    });
}