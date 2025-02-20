using Microsoft.AspNetCore.Builder;
using BuildingBlocks.DependencyInjection;
using MassTransit.Configuration;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notifications.Diagnostics;
using Notifications.Infrastructure.Hubs;

namespace Notifications;

public static class NotificationsModule
{
    public static WebApplicationBuilder Install(WebApplicationBuilder builder)
    {
        builder.AddAssemblyServices(typeof(NotificationsModule).Assembly);

        builder.Services.RegisterConsumer<TodoCompletedEventConsumer>();

        builder.ConfigureMail();

        builder.Services.AddOpenTelemetry().WithTracing(x => x.AddSource(Instrumentation.ServiceName));

        return builder;
    }

    public static WebApplication Map(WebApplication app)
    {
        app.MapHub<TodoHub>("hubs/todo");

        return app;
    }

    private static void ConfigureMail(this WebApplicationBuilder applicationBuilder)
    {
        var mail = applicationBuilder.Configuration.GetConnectionString("mail");

        var smtpUri = new Uri(mail!);

        applicationBuilder.Services
            .AddFluentEmail("fromemail@test.test")
            .AddSmtpSender(smtpUri.Host, smtpUri.Port);
    }
}