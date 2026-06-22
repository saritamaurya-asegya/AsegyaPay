using AsegyaPay.SharedKernel.Domain;
using AsegyaPay.PaymentService.Domain.ValueObjects;
using AsegyaPay.PaymentService.Domain.Enums;

namespace AsegyaPay.PaymentService.Domain.Entities;

/// <summary>
/// Represents a refund against a payment.
/// </summary>
public sealed class Refund : Entity<Guid>
{
    public Guid PaymentId { get; private set; }
    public string MerchantId { get; private set; }
    public Money Amount { get; private set; }
    public string Reason { get; private set; }
    public RefundStatus Status { get; private set; }
    public string InitiatedBy { get; private set; }
    public string? GatewayRefundId { get; private set; }
    public string? FailureReason { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }

    private Refund() : base(Guid.NewGuid()) { }

    internal static Refund Create(Guid paymentId, string merchantId, Money amount, string reason, string initiatedBy)
    {
        return new Refund
        {
            Id = Guid.NewGuid(),
            PaymentId = paymentId,
            MerchantId = merchantId,
            Amount = amount,
            Reason = reason,
            Status = RefundStatus.Pending,
            InitiatedBy = initiatedBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void MarkProcessed(string gatewayRefundId)
    {
        GatewayRefundId = gatewayRefundId;
        Status = RefundStatus.Processed;
        ProcessedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkFailed(string reason)
    {
        FailureReason = reason;
        Status = RefundStatus.Failed;
        UpdatedAt = DateTime.UtcNow;
    }
}
