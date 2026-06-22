using AsegyaPay.SharedKernel.Domain;

namespace AsegyaPay.MerchantService.Domain.Entities;

/// <summary>
/// Merchant aggregate root — represents a business entity registered on the platform.
/// </summary>
public sealed class Merchant : AggregateRoot<Guid>
{
    private readonly List<ApiKey> _apiKeys = [];
    private readonly List<Webhook> _webhooks = [];

    public string BusinessName { get; private set; }
    public string BusinessEmail { get; private set; }
    public string? BusinessPhone { get; private set; }
    public string BusinessType { get; private set; }
    public string? Website { get; private set; }
    public string? Description { get; private set; }
    public MerchantStatus Status { get; private set; }
    public KycStatus KycStatus { get; private set; }
    public string? PanNumber { get; private set; }
    public string? GstNumber { get; private set; }
    public BankAccount? SettlementBankAccount { get; private set; }
    public string Country { get; private set; }
    public string? LogoUrl { get; private set; }
    public decimal SettlementCycleInDays { get; private set; }
    public decimal MdrRate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? ApprovedAt { get; private set; }

    public IReadOnlyList<ApiKey> ApiKeys => _apiKeys.AsReadOnly();
    public IReadOnlyList<Webhook> Webhooks => _webhooks.AsReadOnly();

    private Merchant() : base(Guid.NewGuid()) { }

    public static Merchant Create(
        string businessName,
        string businessEmail,
        string businessType,
        string country,
        string? businessPhone = null,
        string? website = null)
    {
        return new Merchant
        {
            Id = Guid.NewGuid(),
            BusinessName = businessName,
            BusinessEmail = businessEmail.ToLowerInvariant(),
            BusinessType = businessType,
            Country = country,
            BusinessPhone = businessPhone,
            Website = website,
            Status = MerchantStatus.Pending,
            KycStatus = KycStatus.NotSubmitted,
            SettlementCycleInDays = 2,
            MdrRate = 2.0m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public ApiKey GenerateApiKey(string name, ApiKeyType type, string createdBy)
    {
        var apiKey = ApiKey.Create(Id.ToString(), name, type, createdBy);
        _apiKeys.Add(apiKey);
        UpdatedAt = DateTime.UtcNow;
        return apiKey;
    }

    public void RevokeApiKey(Guid apiKeyId)
    {
        var key = _apiKeys.FirstOrDefault(k => k.Id == apiKeyId)
            ?? throw new InvalidOperationException($"API key {apiKeyId} not found.");
        key.Revoke();
        UpdatedAt = DateTime.UtcNow;
    }

    public Webhook AddWebhook(string url, IEnumerable<string> events)
    {
        var webhook = Webhook.Create(Id.ToString(), url, events);
        _webhooks.Add(webhook);
        UpdatedAt = DateTime.UtcNow;
        return webhook;
    }

    public void SubmitKyc(string panNumber, string gstNumber)
    {
        PanNumber = panNumber;
        GstNumber = gstNumber;
        KycStatus = KycStatus.Submitted;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Approve(decimal mdrRate = 2.0m, decimal settlementCycle = 2)
    {
        Status = MerchantStatus.Active;
        KycStatus = KycStatus.Verified;
        MdrRate = mdrRate;
        SettlementCycleInDays = settlementCycle;
        ApprovedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Suspend(string reason)
    {
        Status = MerchantStatus.Suspended;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum MerchantStatus
{
    Pending = 0,
    Active = 1,
    Suspended = 2,
    Terminated = 3
}

public enum KycStatus
{
    NotSubmitted = 0,
    Submitted = 1,
    UnderReview = 2,
    Verified = 3,
    Rejected = 4
}

public enum ApiKeyType
{
    Test = 0,
    Live = 1
}
