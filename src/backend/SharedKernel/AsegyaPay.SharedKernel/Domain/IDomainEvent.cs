using MediatR;

namespace AsegyaPay.SharedKernel.Domain;

/// <summary>
/// Marker interface for domain events.
/// Domain events represent things that happened in the domain.
/// </summary>
public interface IDomainEvent : INotification
{
    Guid EventId { get; }
    DateTime OccurredOn { get; }
}
