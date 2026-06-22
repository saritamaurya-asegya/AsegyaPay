using AsegyaPay.SharedKernel.Domain;

namespace AsegyaPay.PayoutService.Domain.Entities;

/// <summary>
/// Payout aggregate — represents an outward payment to a beneficiary.
/// </summary>
public sealed class Payout : AggregateRoot<Guid>
{
    public string MerchantId { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }
    public PayoutMode Mode { get; private set; }
    public PayoutStatus Status { get; private set; }
    public BeneficiaryInfo Beneficiary { get; private set; }
    public string? Reference { get; private set; }
    public string? Narration { get; private set; }
    public string? UtrNumber { get; private set; }
    public string? FailureReason { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public DateTime? ScheduledAt { get; private set; }

    private Payout() : base(Guid.NewGuid()) { }

    public static Payout Create(
        string merchantId,
        decimal amount,
        string currency,
        PayoutMode mode,
        BeneficiaryInfo beneficiary,
        string? reference = null,
        string? narration = null,
        DateTime? scheduledAt = null)
    {
        return new Payout
        {
            Id = Guid.NewGuid(),
            MerchantId = merchantId,
            Amount = amount,
            Currency = currency,
            Mode = mode,
            Beneficiary = beneficiary,
            Status = scheduledAt.HasValue ? PayoutStatus.Scheduled : PayoutStatus.Pending,
            Reference = reference,
            Narration = narration,
            ScheduledAt = scheduledAt,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void MarkProcessed(string utrNumber)
    {
        UtrNumber = utrNumber;
        Status = PayoutStatus.Processed;
        ProcessedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkFailed(string reason)
    {
        FailureReason = reason;
        Status = PayoutStatus.Failed;
        UpdatedAt = DateTime.UtcNow;
    }
}

public record BeneficiaryInfo(
    string Name,
    string? AccountNumber,
    string? IfscCode,
    string? UpiId,
    string? WalletId);

public enum PayoutMode { BankTransfer, UPI, Wallet, Card }

public enum PayoutStatus
{
    Pending,
    Scheduled,
    Processing,
    Processed,
    Failed,
    Cancelled
}
