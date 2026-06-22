using AsegyaPay.SharedKernel.Domain;

namespace AsegyaPay.SettlementService.Domain.Entities;

/// <summary>
/// Settlement aggregate — represents a settled batch of transactions.
/// </summary>
public sealed class Settlement : AggregateRoot<Guid>
{
    public string MerchantId { get; private set; }
    public decimal GrossAmount { get; private set; }
    public decimal Mdr { get; private set; }
    public decimal Gst { get; private set; }
    public decimal NetAmount { get; private set; }
    public string Currency { get; private set; }
    public SettlementStatus Status { get; private set; }
    public int TransactionCount { get; private set; }
    public DateTime PeriodStart { get; private set; }
    public DateTime PeriodEnd { get; private set; }
    public string? UtrNumber { get; private set; }
    public string? BankReference { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? SettledAt { get; private set; }

    private Settlement() : base(Guid.NewGuid()) { }

    public static Settlement Create(
        string merchantId,
        decimal grossAmount,
        decimal mdrRate,
        string currency,
        int transactionCount,
        DateTime periodStart,
        DateTime periodEnd)
    {
        var mdr = grossAmount * mdrRate;
        var gst = mdr * 0.18m;
        var netAmount = grossAmount - mdr - gst;

        return new Settlement
        {
            Id = Guid.NewGuid(),
            MerchantId = merchantId,
            GrossAmount = grossAmount,
            Mdr = mdr,
            Gst = gst,
            NetAmount = netAmount,
            Currency = currency,
            Status = SettlementStatus.Pending,
            TransactionCount = transactionCount,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void MarkInitiated()
    {
        Status = SettlementStatus.Processing;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkSettled(string utrNumber, string? bankReference = null)
    {
        UtrNumber = utrNumber;
        BankReference = bankReference;
        Status = SettlementStatus.Settled;
        SettledAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkFailed()
    {
        Status = SettlementStatus.Failed;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum SettlementStatus
{
    Pending,
    Processing,
    Settled,
    Failed
}
