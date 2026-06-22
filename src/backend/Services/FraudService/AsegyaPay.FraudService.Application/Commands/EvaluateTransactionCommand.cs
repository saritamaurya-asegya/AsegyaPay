using AsegyaPay.FraudService.Domain.Entities;
using AsegyaPay.FraudService.Application.Interfaces;
using AsegyaPay.SharedKernel.Application;
using AsegyaPay.SharedKernel.Common;

namespace AsegyaPay.FraudService.Application.Commands;

// ── Command ──────────────────────────────────────────────────────────────────

public sealed record EvaluateTransactionCommand(
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
    Dictionary<string, string>? Metadata
) : ICommand<Result<FraudAssessmentResult>>;

public sealed record FraudAssessmentResult(
    Guid AssessmentId,
    int RiskScore,
    string RiskLevel,
    string Decision,
    List<string> TriggeredRules,
    long ProcessingTimeMs
);

// ── Handler ───────────────────────────────────────────────────────────────────

public sealed class EvaluateTransactionCommandHandler
    : ICommandHandler<EvaluateTransactionCommand, Result<FraudAssessmentResult>>
{
    private readonly IFraudRuleEngine _ruleEngine;
    private readonly IRiskScorer _riskScorer;
    private readonly IIpIntelligenceService _ipIntelligence;
    private readonly IVelocityChecker _velocityChecker;

    public EvaluateTransactionCommandHandler(
        IFraudRuleEngine ruleEngine,
        IRiskScorer riskScorer,
        IIpIntelligenceService ipIntelligence,
        IVelocityChecker velocityChecker)
    {
        _ruleEngine = ruleEngine;
        _riskScorer = riskScorer;
        _ipIntelligence = ipIntelligence;
        _velocityChecker = velocityChecker;
    }

    public async Task<Result<FraudAssessmentResult>> Handle(
        EvaluateTransactionCommand request,
        CancellationToken cancellationToken)
    {
        var startTime = DateTime.UtcNow;

        var fraudRequest = new FraudEvaluationRequest(
            PaymentId: request.PaymentId,
            MerchantId: request.MerchantId,
            CustomerId: request.CustomerId,
            Amount: request.Amount,
            Currency: request.Currency,
            PaymentMethod: request.PaymentMethod,
            IpAddress: request.IpAddress,
            DeviceFingerprint: request.DeviceFingerprint,
            UserAgent: request.UserAgent,
            CardBin: request.CardBin,
            Metadata: request.Metadata);

        var (mlScore, ruleResult, ipResult, velocityResult) = await (
            _riskScorer.ScoreAsync(fraudRequest, cancellationToken),
            _ruleEngine.EvaluateAsync(fraudRequest, cancellationToken),
            request.IpAddress != null
                ? _ipIntelligence.AnalyzeAsync(request.IpAddress, cancellationToken)
                : Task.FromResult(new IpIntelligenceResult(null, false, false, false, null)),
            _velocityChecker.CheckAsync(request.MerchantId, request.CustomerId, request.IpAddress, request.Amount, cancellationToken)
        );

        var combinedScore = Math.Min(1000, mlScore + ruleResult.AdditionalScore);

        var context = new FraudContext(
            IpAddress: request.IpAddress,
            DeviceFingerprint: request.DeviceFingerprint,
            UserAgent: request.UserAgent,
            Country: ipResult.Country,
            IsVpn: ipResult.IsVpn,
            IsProxy: ipResult.IsProxy,
            IsTor: ipResult.IsTor,
            CardBin: request.CardBin,
            CardCountry: null,
            CardType: null);

        var assessment = FraudAssessment.Create(
            paymentId: request.PaymentId,
            merchantId: request.MerchantId,
            amount: request.Amount,
            currency: request.Currency,
            riskScore: combinedScore,
            triggeredRules: ruleResult.TriggeredRules,
            context: context);

        var processingTimeMs = (long)(DateTime.UtcNow - startTime).TotalMilliseconds;

        return Result.Success(new FraudAssessmentResult(
            AssessmentId: assessment.Id,
            RiskScore: assessment.RiskScore,
            RiskLevel: assessment.RiskLevel.ToString(),
            Decision: assessment.Decision.ToString(),
            TriggeredRules: assessment.TriggeredRules,
            ProcessingTimeMs: processingTimeMs));
    }
}
