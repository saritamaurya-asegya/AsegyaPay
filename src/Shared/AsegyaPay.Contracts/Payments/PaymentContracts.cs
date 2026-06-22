namespace AsegyaPay.Contracts.Payments;

/// <summary>
/// Request to create a new payment.
/// </summary>
public record CreatePaymentRequest
{
    public required decimal Amount { get; init; }
    public required string Currency { get; init; }
    public required string Method { get; init; }
    public string? Description { get; init; }
    public string? OrderId { get; init; }
    public string? CustomerId { get; init; }
    public string? ReturnUrl { get; init; }
    public string? CallbackUrl { get; init; }
    public Dictionary<string, string>? Metadata { get; init; }
    public PaymentMethodDetails? MethodDetails { get; init; }
}

public record PaymentMethodDetails
{
    public CardDetails? Card { get; init; }
    public UpiDetails? Upi { get; init; }
    public NetBankingDetails? NetBanking { get; init; }
    public WalletDetails? Wallet { get; init; }
}

public record CardDetails
{
    public string? Token { get; init; }
    public string? Last4 { get; init; }
    public string? Network { get; init; }
    public string? Type { get; init; }
    public bool SaveCard { get; init; }
}

public record UpiDetails
{
    public string? VirtualPaymentAddress { get; init; }
    public string? Flow { get; init; } // collect, intent, qr
}

public record NetBankingDetails
{
    public required string BankCode { get; init; }
}

public record WalletDetails
{
    public required string Provider { get; init; } // paytm, phonepe, amazonpay
}

/// <summary>
/// Payment response returned to the client.
/// </summary>
public record PaymentResponse
{
    public required string Id { get; init; }
    public required string MerchantId { get; init; }
    public required decimal Amount { get; init; }
    public required string Currency { get; init; }
    public required string Status { get; init; }
    public required string Method { get; init; }
    public string? Description { get; init; }
    public string? OrderId { get; init; }
    public string? ReferenceId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public string? FailureReason { get; init; }
    public Dictionary<string, string>? Metadata { get; init; }
}

/// <summary>
/// Request to capture an authorized payment.
/// </summary>
public record CapturePaymentRequest
{
    public required string PaymentId { get; init; }
    public decimal? Amount { get; init; } // For partial capture
}

/// <summary>
/// Request to refund a payment.
/// </summary>
public record RefundRequest
{
    public required string PaymentId { get; init; }
    public required decimal Amount { get; init; }
    public string? Reason { get; init; }
    public Dictionary<string, string>? Metadata { get; init; }
}

public record RefundResponse
{
    public required string Id { get; init; }
    public required string PaymentId { get; init; }
    public required decimal Amount { get; init; }
    public required string Status { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ProcessedAt { get; init; }
}

/// <summary>
/// Payment status enum values.
/// </summary>
public static class PaymentStatus
{
    public const string Created = "created";
    public const string Authorized = "authorized";
    public const string Captured = "captured";
    public const string Failed = "failed";
    public const string Refunded = "refunded";
    public const string PartiallyRefunded = "partially_refunded";
    public const string Cancelled = "cancelled";
    public const string Processing = "processing";
}
