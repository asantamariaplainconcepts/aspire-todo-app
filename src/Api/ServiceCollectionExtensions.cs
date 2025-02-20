using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Todos.Infrastructure.Persistence;

namespace Api;

public static class ServiceCollectionExtensions
{
    public static void AddCustomMasstransit(this WebApplicationBuilder builder)
    {
        var host = builder.Configuration.GetConnectionString("queue");

        builder.Services.AddMassTransit(busConfigurator =>
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

    public static WebApplicationBuilder AddCustomSeqEndpoint(this WebApplicationBuilder builder)
    {
        if (builder.Configuration.GetConnectionString("seq") != null)
        {
            builder.AddSeqEndpoint("seq");
        }

        return builder;
    }
}