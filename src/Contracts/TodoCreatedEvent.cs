using MediatR;

namespace Contracts;

public sealed record TodoCreatedEvent(Guid TodoId) : INotification;