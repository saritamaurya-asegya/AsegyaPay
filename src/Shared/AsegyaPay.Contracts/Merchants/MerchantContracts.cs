namespace AsegyaPay.Contracts.Merchants;

/// <summary>
/// Request to register a new merchant.
/// </summary>
public record MerchantRegistrationRequest
{
    public required string BusinessName { get; init; }
    public required string Email { get; init; }
    public required string Phone { get; init; }
    public required string BusinessType { get; init; } // individual, partnership, company, llp
    public required string Category { get; init; }
    public string? Website { get; init; }
    public required AddressDto Address { get; init; }
    public required ContactPersonDto ContactPerson { get; init; }
}

public record AddressDto
{
    public required string Line1 { get; init; }
    public string? Line2 { get; init; }
    public required string City { get; init; }
    public required string State { get; init; }
    public required string Country { get; init; }
    public required string PostalCode { get; init; }
}

public record ContactPersonDto
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required string Phone { get; init; }
    public string? Designation { get; init; }
}

/// <summary>
/// Merchant profile response.
/// </summary>
public record MerchantResponse
{
    public required string Id { get; init; }
    public required string BusinessName { get; init; }
    public required string Email { get; init; }
    public required string Status { get; init; } // pending, active, suspended, terminated
    public required string BusinessType { get; init; }
    public string? MerchantCode { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ActivatedAt { get; init; }
    public MerchantSettingsDto? Settings { get; init; }
}

public record MerchantSettingsDto
{
    public bool AutoCapture { get; init; } = true;
    public string DefaultCurrency { get; init; } = "INR";
    public List<string> EnabledMethods { get; init; } = new();
    public WebhookConfigDto? WebhookConfig { get; init; }
}

public record WebhookConfigDto
{
    public string? Url { get; init; }
    public string? Secret { get; init; }
    public List<string> Events { get; init; } = new();
    public bool IsActive { get; init; }
}

public static class MerchantStatus
{
    public const string Pending = "pending";
    public const string UnderReview = "under_review";
    public const string Active = "active";
    public const string Suspended = "suspended";
    public const string Terminated = "terminated";
}
