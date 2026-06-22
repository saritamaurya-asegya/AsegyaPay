using AsegyaPay.FraudService.Domain.Entities;

namespace AsegyaPay.FraudService.Application.Interfaces;

/// <summary>
/// Fraud rule engine — evaluates a transaction against all configured fraud rules.
/// </summary>
public interface IFraudRuleEngine
{
    Task<RuleEngineResult> EvaluateAsync(FraudEvaluationRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// ML-based risk scorer — returns a risk score 0-1000.
/// </summary>
public interface IRiskScorer
{
    Task<int> ScoreAsync(FraudEvaluationRequest request, CancellationToken cancellationToken = default);
}

/// <summary>
/// IP intelligence service — checks VPN, proxy, TOR, geo-location.
/// </summary>
public interface IIpIntelligenceService
{
    Task<IpIntelligenceResult> AnalyzeAsync(string ipAddress, CancellationToken cancellationToken = default);
}

/// <summary>
/// Velocity checker — detects velocity-based fraud patterns.
/// </summary>
public interface IVelocityChecker
{
    Task<VelocityCheckResult> CheckAsync(string merchantId, string? customerId, string? ipAddress, decimal amount, CancellationToken cancellationToken = default);
}

public record FraudEvaluationRequest(
    Guid PaymentId,
    string MerchantId,
    string? CustomerId,
    decimal Amount,
    string Currency,
    string PaymentMethod,
    string? IpAddress,
    string? DeviceFingerprint,
    string? UserAgent,
    string? CardBin,
    Dictionary<string, string>? Metadata);

public record RuleEngineResult(
    bool IsBlocked,
    List<string> TriggeredRules,
    int AdditionalScore);

public record IpIntelligenceResult(
    string? Country,
    bool IsVpn,
    bool IsProxy,
    bool IsTor,
    double? RiskScore);

public record VelocityCheckResult(
    bool IsHighVelocity,
    int TransactionCount1Min,
    int TransactionCount1Hour,
    decimal TotalAmount1Hour);
