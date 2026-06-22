using AsegyaPay.Common.Models;

namespace PaymentService.Domain.Entities;

/// <summary>
/// Payment aggregate root - the core entity of the payment service.
/// </summary>
public class Payment : AggregateRoot
{
    public string MerchantId { get; private set; } = null!;
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = null!;
    public string Status { get; private set; } = null!;
    public string Method { get; private set; } = null!;
    public string? Description { get; private set; }
    public string? OrderId { get; private set; }
    public string? CustomerId { get; private set; }
    public string? ReferenceId { get; private set; }
    public string? GatewayTransactionId { get; private set; }
    public string? FailureReason { get; private set; }
    public string? ReturnUrl { get; private set; }
    public string? CallbackUrl { get; private set; }
    public decimal? RefundedAmount { get; private set; }
    public DateTime? AuthorizedAt { get; private set; }
    public DateTime? CapturedAt { get; private set; }
    public DateTime? FailedAt { get; private set; }
    public int AttemptCount { get; private set; } = 0;
    public double? RiskScore { get; private set; }

    public ICollection<PaymentAttempt> Attempts { get; private set; } = new List<PaymentAttempt>();
    public ICollection<Refund> Refunds { get; private set; } = new List<Refund>();

    private Payment() { }

    public static Payment Create(
        string merchantId,
        decimal amount,
        string currency,
        string method,
        string? description = null,
        string? orderId = null,
        string? customerId = null,
        string? returnUrl = null,
        string? callbackUrl = null)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive.", nameof(amount));
        if (string.IsNullOrWhiteSpace(merchantId)) throw new ArgumentException("MerchantId is required.", nameof(merchantId));

        var payment = new Payment
        {
            MerchantId = merchantId,
            Amount = amount,
            Currency = currency.ToUpperInvariant(),
            Status = PaymentState.Created,
            Method = method,
            Description = description,
            OrderId = orderId,
            CustomerId = customerId,
            ReturnUrl = returnUrl,
            CallbackUrl = callbackUrl,
            ReferenceId = GenerateReferenceId()
        };

        payment.AddDomainEvent(new PaymentCreatedDomainEvent(payment.Id, merchantId, amount, currency, method));
        return payment;
    }

    public void Authorize(string gatewayTransactionId)
    {
        if (Status != PaymentState.Created && Status != PaymentState.Processing)
            throw new InvalidOperationException($"Cannot authorize payment in '{Status}' state.");

        Status = PaymentState.Authorized;
        GatewayTransactionId = gatewayTransactionId;
        AuthorizedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new PaymentAuthorizedDomainEvent(Id, MerchantId, Amount));
    }

    public void Capture(decimal? captureAmount = null)
    {
        if (Status != PaymentState.Authorized)
            throw new InvalidOperationException($"Cannot capture payment in '{Status}' state.");

        if (captureAmount.HasValue && captureAmount.Value > Amount)
            throw new ArgumentException("Capture amount cannot exceed payment amount.");

        Status = PaymentState.Captured;
        CapturedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new PaymentCapturedDomainEvent(Id, MerchantId, captureAmount ?? Amount));
    }

    public void Fail(string reason)
    {
        Status = PaymentState.Failed;
        FailureReason = reason;
        FailedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new PaymentFailedDomainEvent(Id, MerchantId, reason));
    }

    public Refund InitiateRefund(decimal amount, string? reason = null)
    {
        if (Status != PaymentState.Captured && Status != PaymentState.PartiallyRefunded)
            throw new InvalidOperationException($"Cannot refund payment in '{Status}' state.");

        var totalRefunded = (RefundedAmount ?? 0) + amount;
        if (totalRefunded > Amount)
            throw new InvalidOperationException("Refund amount exceeds payment amount.");

        var refund = Refund.Create(Id, amount, reason);
        Refunds.Add(refund);
        RefundedAmount = totalRefunded;
        Status = totalRefunded >= Amount ? PaymentState.Refunded : PaymentState.PartiallyRefunded;
        UpdatedAt = DateTime.UtcNow;

        return refund;
    }

    public void SetRiskScore(double score)
    {
        RiskScore = score;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkProcessing()
    {
        Status = PaymentState.Processing;
        AttemptCount++;
        UpdatedAt = DateTime.UtcNow;
    }

    private static string GenerateReferenceId()
    {
        return $"pay_{Guid.NewGuid():N}"[..24];
    }
}

public static class PaymentState
{
    public const string Created = "created";
    public const string Processing = "processing";
    public const string Authorized = "authorized";
    public const string Captured = "captured";
    public const string Failed = "failed";
    public const string Refunded = "refunded";
    public const string PartiallyRefunded = "partially_refunded";
    public const string Cancelled = "cancelled";
}

public class PaymentAttempt : BaseEntity
{
    public Guid PaymentId { get; private set; }
    public string Method { get; private set; } = null!;
    public string Status { get; private set; } = null!;
    public string? GatewayResponse { get; private set; }
    public string? ErrorCode { get; private set; }
    public string? ErrorMessage { get; private set; }
    public DateTime AttemptedAt { get; private set; } = DateTime.UtcNow;

    private PaymentAttempt() { }

    public static PaymentAttempt Create(Guid paymentId, string method)
    {
        return new PaymentAttempt
        {
            PaymentId = paymentId,
            Method = method,
            Status = "initiated"
        };
    }
}

public class Refund : BaseEntity
{
    public Guid PaymentId { get; private set; }
    public decimal Amount { get; private set; }
    public string Status { get; private set; } = null!;
    public string? Reason { get; private set; }
    public string? ReferenceId { get; private set; }
    public DateTime? ProcessedAt { get; private set; }

    private Refund() { }

    public static Refund Create(Guid paymentId, decimal amount, string? reason)
    {
        return new Refund
        {
            PaymentId = paymentId,
            Amount = amount,
            Status = "initiated",
            Reason = reason,
            ReferenceId = $"rfnd_{Guid.NewGuid():N}"[..24]
        };
    }

    public void MarkProcessed()
    {
        Status = "processed";
        ProcessedAt = DateTime.UtcNow;
    }

    public void MarkFailed(string reason)
    {
        Status = "failed";
        Reason = reason;
    }
}

#region Domain Events

public record PaymentCreatedDomainEvent(Guid PaymentId, string MerchantId, decimal Amount, string Currency, string Method) : DomainEvent
{
    public override string EventType => "payment.created";
}

public record PaymentAuthorizedDomainEvent(Guid PaymentId, string MerchantId, decimal Amount) : DomainEvent
{
    public override string EventType => "payment.authorized";
}

public record PaymentCapturedDomainEvent(Guid PaymentId, string MerchantId, decimal Amount) : DomainEvent
{
    public override string EventType => "payment.captured";
}

public record PaymentFailedDomainEvent(Guid PaymentId, string MerchantId, string Reason) : DomainEvent
{
    public override string EventType => "payment.failed";
}

#endregion
