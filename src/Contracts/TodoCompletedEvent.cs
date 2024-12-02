using MediatR;

namespace Contracts;

public record TodoCompletedEvent(Guid Id, string Title) : INotification;