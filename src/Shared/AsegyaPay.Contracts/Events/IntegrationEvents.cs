namespace AsegyaPay.Contracts.Events;

/// <summary>
/// Integration events for inter-service communication via Kafka/RabbitMQ.
/// </summary>
public record PaymentCreatedEvent
{
    public required string PaymentId { get; init; }
    public required string MerchantId { get; init; }
    public required decimal Amount { get; init; }
    public required string Currency { get; init; }
    public required string Method { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

public record PaymentCompletedEvent
{
    public required string PaymentId { get; init; }
    public required string MerchantId { get; init; }
    public required decimal Amount { get; init; }
    public required string Currency { get; init; }
    public required string Method { get; init; }
    public required string Status { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

public record PaymentFailedEvent
{
    public required string PaymentId { get; init; }
    public required string MerchantId { get; init; }
    public required string Reason { get; init; }
    public string? ErrorCode { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

public record RefundInitiatedEvent
{
    public required string RefundId { get; init; }
    public required string PaymentId { get; init; }
    public required decimal Amount { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

public record MerchantOnboardedEvent
{
    public required string MerchantId { get; init; }
    public required string BusinessName { get; init; }
    public required string Email { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

public record FraudAlertEvent
{
    public required string PaymentId { get; init; }
    public required string MerchantId { get; init; }
    public required double RiskScore { get; init; }
    public required string AlertType { get; init; }
    public required string Reason { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

public record SettlementProcessedEvent
{
    public required string SettlementId { get; init; }
    public required string MerchantId { get; init; }
    public required decimal Amount { get; init; }
    public required string Currency { get; init; }
    public required int TransactionCount { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
