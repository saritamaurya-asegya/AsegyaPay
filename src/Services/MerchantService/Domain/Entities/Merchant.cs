using AsegyaPay.Common.Models;

namespace MerchantService.Domain.Entities;

/// <summary>
/// Merchant aggregate root.
/// </summary>
public class Merchant : AggregateRoot
{
    public string BusinessName { get; private set; } = null!;
    public string? MerchantCode { get; private set; }
    public string Email { get; private set; } = null!;
    public string Phone { get; private set; } = null!;
    public string Status { get; private set; } = null!;
    public string BusinessType { get; private set; } = null!;
    public string? Category { get; private set; }
    public string? Website { get; private set; }
    public string? LogoUrl { get; private set; }
    public DateTime? ActivatedAt { get; private set; }
    public DateTime? SuspendedAt { get; private set; }

    public ICollection<MerchantAddress> Addresses { get; private set; } = new List<MerchantAddress>();
    public ICollection<MerchantContact> Contacts { get; private set; } = new List<MerchantContact>();
    public ICollection<ApiKey> ApiKeys { get; private set; } = new List<ApiKey>();

    private Merchant() { }

    public static Merchant Create(
        string businessName,
        string email,
        string phone,
        string businessType,
        string? category = null,
        string? website = null)
    {
        return new Merchant
        {
            BusinessName = businessName,
            Email = email,
            Phone = phone,
            BusinessType = businessType,
            Category = category,
            Website = website,
            Status = "pending",
            MerchantCode = GenerateMerchantCode()
        };
    }

    public void Activate()
    {
        Status = "active";
        ActivatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Suspend(string reason)
    {
        Status = "suspended";
        SuspendedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkUnderReview()
    {
        Status = "under_review";
        UpdatedAt = DateTime.UtcNow;
    }

    private static string GenerateMerchantCode()
    {
        return $"MRC{DateTime.UtcNow:yyyyMMdd}{Random.Shared.Next(100000, 999999)}";
    }
}

public class MerchantAddress : BaseEntity
{
    public Guid MerchantId { get; private set; }
    public string AddressType { get; private set; } = "registered";
    public string Line1 { get; private set; } = null!;
    public string? Line2 { get; private set; }
    public string City { get; private set; } = null!;
    public string State { get; private set; } = null!;
    public string Country { get; private set; } = "IND";
    public string PostalCode { get; private set; } = null!;

    private MerchantAddress() { }

    public static MerchantAddress Create(Guid merchantId, string line1, string city, string state, string postalCode, string country = "IND")
    {
        return new MerchantAddress
        {
            MerchantId = merchantId,
            Line1 = line1,
            City = city,
            State = state,
            PostalCode = postalCode,
            Country = country
        };
    }
}

public class MerchantContact : BaseEntity
{
    public Guid MerchantId { get; private set; }
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string Phone { get; private set; } = null!;
    public string? Designation { get; private set; }
    public bool IsPrimary { get; private set; }

    private MerchantContact() { }
}

public class ApiKey : BaseEntity
{
    public Guid MerchantId { get; private set; }
    public string KeyId { get; private set; } = null!;
    public string KeySecretHash { get; private set; } = null!;
    public string? Name { get; private set; }
    public string Environment { get; private set; } = "test";
    public bool IsActive { get; private set; } = true;
    public DateTime? LastUsedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    private ApiKey() { }

    public static ApiKey Create(Guid merchantId, string name, string environment, string keySecretHash)
    {
        return new ApiKey
        {
            MerchantId = merchantId,
            KeyId = $"rzp_{environment[..4]}_{Guid.NewGuid():N}"[..30],
            KeySecretHash = keySecretHash,
            Name = name,
            Environment = environment
        };
    }

    public void Revoke()
    {
        IsActive = false;
        RevokedAt = DateTime.UtcNow;
    }

    public void RecordUsage()
    {
        LastUsedAt = DateTime.UtcNow;
    }
}
