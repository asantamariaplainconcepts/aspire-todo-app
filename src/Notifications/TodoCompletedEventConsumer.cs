using System.Text.Json;
using BuildingBlocks.Diagnostics;
using Contracts;
using FluentEmail.Core;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Notifications.Infrastructure.Hubs;

namespace Notifications;

public class TodoCompletedEventConsumer(
    ILogger<TodoCompletedEventConsumer> logger,
    IFluentEmail fluentEmail,
    IHubContext<TodoHub, ITodoHub> hubContext) : IConsumer<TodoCompletedEvent>
{
    public async Task Consume(ConsumeContext<TodoCompletedEvent> context)
    {
        using var activity = Instrumentation.Source.StartActivity(nameof(TodoCompletedEventConsumer));

        Thread.Sleep(TimeSpan.FromSeconds(3));

        logger.LogInformation("(NOTIFICATIONS) --- Todo {TodoId} has been completed, ",
            context.Message.Id);

        await hubContext.Clients.All.SendCompleted(new TodoCompleted
        {
            Title = context.Message.Title
        });

        await fluentEmail
            .To("hellO@dotnet.com")
            .Subject("Todo Created")
            .Body(JsonSerializer.Serialize(context.Message))
            .SendAsync();

        logger.LogInformation("(NOTIFICATIONS) --- Notification sent to all clients for Todo {TodoId}",
            context.Message.Id);
    }
}