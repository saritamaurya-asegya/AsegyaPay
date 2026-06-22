using AsegyaPay.SharedKernel.Domain;

namespace AsegyaPay.FraudService.Domain.Entities;

/// <summary>
/// Represents a fraud risk assessment for a transaction.
/// </summary>
public sealed class FraudAssessment : AggregateRoot<Guid>
{
    public Guid PaymentId { get; private set; }
    public string MerchantId { get; private set; }
    public string? CustomerId { get; private set; }
    public int RiskScore { get; private set; }
    public RiskLevel RiskLevel { get; private set; }
    public FraudDecision Decision { get; private set; }
    public List<string> TriggeredRules { get; private set; } = [];
    public string? IpAddress { get; private set; }
    public string? DeviceFingerprint { get; private set; }
    public string? UserAgent { get; private set; }
    public string? Country { get; private set; }
    public bool IsVpn { get; private set; }
    public bool IsProxy { get; private set; }
    public bool IsTor { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }
    public string? CardBin { get; private set; }
    public string? CardCountry { get; private set; }
    public string? CardType { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public long ProcessingTimeMs { get; private set; }

    private FraudAssessment() : base(Guid.NewGuid()) { }

    public static FraudAssessment Create(
        Guid paymentId,
        string merchantId,
        decimal amount,
        string currency,
        int riskScore,
        List<string> triggeredRules,
        FraudContext context)
    {
        var riskLevel = riskScore switch
        {
            <= 300 => RiskLevel.Low,
            <= 600 => RiskLevel.Medium,
            <= 800 => RiskLevel.High,
            _ => RiskLevel.Critical
        };

        var decision = riskLevel switch
        {
            RiskLevel.Low => FraudDecision.Allow,
            RiskLevel.Medium => FraudDecision.Review,
            RiskLevel.High => FraudDecision.Challenge,
            RiskLevel.Critical => FraudDecision.Block,
            _ => FraudDecision.Allow
        };

        return new FraudAssessment
        {
            Id = Guid.NewGuid(),
            PaymentId = paymentId,
            MerchantId = merchantId,
            Amount = amount,
            Currency = currency,
            RiskScore = riskScore,
            RiskLevel = riskLevel,
            Decision = decision,
            TriggeredRules = triggeredRules,
            IpAddress = context.IpAddress,
            DeviceFingerprint = context.DeviceFingerprint,
            UserAgent = context.UserAgent,
            Country = context.Country,
            IsVpn = context.IsVpn,
            IsProxy = context.IsProxy,
            IsTor = context.IsTor,
            CardBin = context.CardBin,
            CardCountry = context.CardCountry,
            CardType = context.CardType,
            CreatedAt = DateTime.UtcNow
        };
    }
}

public record FraudContext(
    string? IpAddress,
    string? DeviceFingerprint,
    string? UserAgent,
    string? Country,
    bool IsVpn,
    bool IsProxy,
    bool IsTor,
    string? CardBin,
    string? CardCountry,
    string? CardType);

public enum RiskLevel { Low, Medium, High, Critical }

public enum FraudDecision
{
    Allow,
    Review,
    Challenge,
    Block
}
