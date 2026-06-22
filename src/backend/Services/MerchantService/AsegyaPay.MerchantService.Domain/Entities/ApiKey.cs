using AsegyaPay.SharedKernel.Domain;
using System.Security.Cryptography;

namespace AsegyaPay.MerchantService.Domain.Entities;

/// <summary>
/// API Key entity — used for merchant API authentication.
/// Only the prefix is stored in plaintext; the full key is hashed.
/// </summary>
public sealed class ApiKey : Entity<Guid>
{
    public string MerchantId { get; private set; }
    public string Name { get; private set; }
    public ApiKeyType Type { get; private set; }
    public string Prefix { get; private set; }
    public string KeyHash { get; private set; }
    public bool IsRevoked { get; private set; }
    public string CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public DateTime? LastUsedAt { get; private set; }

    private ApiKey() : base(Guid.NewGuid()) { }

    internal static ApiKey Create(string merchantId, string name, ApiKeyType type, string createdBy)
    {
        var rawKey = GenerateRawKey(type);
        var prefix = rawKey[..12];
        var hash = HashKey(rawKey);

        return new ApiKey
        {
            Id = Guid.NewGuid(),
            MerchantId = merchantId,
            Name = name,
            Type = type,
            Prefix = prefix,
            KeyHash = hash,
            IsRevoked = false,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Revoke()
    {
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
    }

    public void RecordUsage()
    {
        LastUsedAt = DateTime.UtcNow;
    }

    private static string GenerateRawKey(ApiKeyType type)
    {
        var prefix = type == ApiKeyType.Test ? "test" : "live";
        var random = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLowerInvariant();
        return $"rzp_{prefix}_{random}";
    }

    private static string HashKey(string rawKey)
    {
        return Convert.ToHexString(
            System.Security.Cryptography.SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes(rawKey)));
    }
}

/// <summary>
/// Webhook configuration for a merchant.
/// </summary>
public sealed class Webhook : Entity<Guid>
{
    public string MerchantId { get; private set; }
    public string Url { get; private set; }
    public List<string> Events { get; private set; }
    public string Secret { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Webhook() : base(Guid.NewGuid()) { }

    internal static Webhook Create(string merchantId, string url, IEnumerable<string> events)
    {
        return new Webhook
        {
            Id = Guid.NewGuid(),
            MerchantId = merchantId,
            Url = url,
            Events = events.ToList(),
            Secret = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Deactivate() => IsActive = false;
}

/// <summary>
/// Bank account value object for settlement.
/// </summary>
public sealed class BankAccount
{
    public string AccountNumber { get; init; }
    public string IfscCode { get; init; }
    public string BankName { get; init; }
    public string AccountHolderName { get; init; }

    public BankAccount(string accountNumber, string ifscCode, string bankName, string accountHolderName)
    {
        AccountNumber = accountNumber;
        IfscCode = ifscCode;
        BankName = bankName;
        AccountHolderName = accountHolderName;
    }
}
