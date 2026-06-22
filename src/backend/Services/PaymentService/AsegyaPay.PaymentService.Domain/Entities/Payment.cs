using AsegyaPay.SharedKernel.Domain;
using AsegyaPay.PaymentService.Domain.Enums;
using AsegyaPay.PaymentService.Domain.ValueObjects;
using AsegyaPay.PaymentService.Domain.Events;

namespace AsegyaPay.PaymentService.Domain.Entities;

/// <summary>
/// Payment aggregate root — represents the lifecycle of a single payment transaction.
/// </summary>
public sealed class Payment : AggregateRoot<Guid>
{
    private readonly List<Refund> _refunds = [];

    public string OrderId { get; private set; }
    public string MerchantId { get; private set; }
    public string? CustomerId { get; private set; }
    public Money Amount { get; private set; }
    public Money? CapturedAmount { get; private set; }
    public Money RefundedAmount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public PaymentMethod Method { get; private set; }
    public PaymentGateway Gateway { get; private set; }
    public string? GatewayTransactionId { get; private set; }
    public string? GatewayOrderId { get; private set; }
    public string Description { get; private set; }
    public string? FailureReason { get; private set; }
    public string? CallbackUrl { get; private set; }
    public string? RedirectUrl { get; private set; }
    public Dictionary<string, string> Metadata { get; private set; }
    public int FraudScore { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? AuthorizedAt { get; private set; }
    public DateTime? CapturedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }

    public IReadOnlyList<Refund> Refunds => _refunds.AsReadOnly();

    private Payment() : base(Guid.NewGuid()) { }

    public static Payment Create(
        string orderId,
        string merchantId,
        Money amount,
        PaymentMethod method,
        string description,
        string? customerId = null,
        string? callbackUrl = null,
        string? redirectUrl = null,
        Dictionary<string, string>? metadata = null)
    {
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            MerchantId = merchantId,
            CustomerId = customerId,
            Amount = amount,
            RefundedAmount = Money.Zero(amount.Currency),
            Status = PaymentStatus.Pending,
            Method = method,
            Gateway = PaymentGateway.Internal,
            Description = description,
            CallbackUrl = callbackUrl,
            RedirectUrl = redirectUrl,
            Metadata = metadata ?? [],
            FraudScore = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30)
        };

        payment.RaiseDomainEvent(new PaymentCreatedEvent(payment.Id, payment.MerchantId, payment.Amount));
        return payment;
    }

    public void Authorize(string gatewayTransactionId, string gatewayOrderId, PaymentGateway gateway)
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException($"Cannot authorize a payment in '{Status}' status.");

        GatewayTransactionId = gatewayTransactionId;
        GatewayOrderId = gatewayOrderId;
        Gateway = gateway;
        Status = PaymentStatus.Authorized;
        AuthorizedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new PaymentAuthorizedEvent(Id, MerchantId, Amount, gatewayTransactionId));
    }

    public void Capture(Money? captureAmount = null)
    {
        if (Status != PaymentStatus.Authorized)
            throw new InvalidOperationException($"Cannot capture a payment in '{Status}' status.");

        var amountToCapture = captureAmount ?? Amount;

        if (amountToCapture.IsGreaterThan(Amount))
            throw new InvalidOperationException("Capture amount cannot exceed authorized amount.");

        CapturedAmount = amountToCapture;
        Status = PaymentStatus.Captured;
        CapturedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new PaymentCapturedEvent(Id, MerchantId, CapturedAmount));
    }

    public Refund Refund(Money refundAmount, string reason, string initiatedBy)
    {
        if (Status != PaymentStatus.Captured && Status != PaymentStatus.PartiallyRefunded)
            throw new InvalidOperationException($"Cannot refund a payment in '{Status}' status.");

        var effectiveAmount = CapturedAmount ?? Amount;
        var availableAmount = effectiveAmount.Subtract(RefundedAmount);

        if (refundAmount.IsGreaterThan(availableAmount))
            throw new InvalidOperationException("Refund amount exceeds available refundable amount.");

        var refund = Entities.Refund.Create(Id, MerchantId, refundAmount, reason, initiatedBy);
        _refunds.Add(refund);

        RefundedAmount = RefundedAmount.Add(refundAmount);

        Status = RefundedAmount == effectiveAmount
            ? PaymentStatus.Refunded
            : PaymentStatus.PartiallyRefunded;

        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new PaymentRefundedEvent(Id, MerchantId, refundAmount, refund.Id));
        return refund;
    }

    public void Fail(string reason)
    {
        if (Status == PaymentStatus.Captured || Status == PaymentStatus.Refunded)
            throw new InvalidOperationException($"Cannot fail a payment in '{Status}' status.");

        FailureReason = reason;
        Status = PaymentStatus.Failed;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new PaymentFailedEvent(Id, MerchantId, Amount, reason));
    }

    public void Cancel()
    {
        if (Status != PaymentStatus.Pending && Status != PaymentStatus.Authorized)
            throw new InvalidOperationException($"Cannot cancel a payment in '{Status}' status.");

        Status = PaymentStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new PaymentCancelledEvent(Id, MerchantId, Amount));
    }

    public void SetFraudScore(int score)
    {
        FraudScore = score;
        UpdatedAt = DateTime.UtcNow;
    }
}
