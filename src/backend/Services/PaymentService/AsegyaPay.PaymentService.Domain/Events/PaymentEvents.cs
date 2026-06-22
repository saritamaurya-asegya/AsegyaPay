using AsegyaPay.SharedKernel.Domain;
using AsegyaPay.PaymentService.Domain.ValueObjects;

namespace AsegyaPay.PaymentService.Domain.Events;

public sealed record PaymentCreatedEvent(
    Guid PaymentId,
    string MerchantId,
    Money Amount) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public sealed record PaymentAuthorizedEvent(
    Guid PaymentId,
    string MerchantId,
    Money Amount,
    string GatewayTransactionId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public sealed record PaymentCapturedEvent(
    Guid PaymentId,
    string MerchantId,
    Money Amount) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public sealed record PaymentRefundedEvent(
    Guid PaymentId,
    string MerchantId,
    Money RefundAmount,
    Guid RefundId) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public sealed record PaymentFailedEvent(
    Guid PaymentId,
    string MerchantId,
    Money Amount,
    string Reason) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public sealed record PaymentCancelledEvent(
    Guid PaymentId,
    string MerchantId,
    Money Amount) : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
