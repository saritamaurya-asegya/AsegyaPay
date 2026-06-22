namespace AsegyaPay.Contracts.Auth;

public record LoginRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public string? MfaCode { get; init; }
}

public record LoginResponse
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
    public required int ExpiresIn { get; init; }
    public required string TokenType { get; init; }
    public MerchantInfo? Merchant { get; init; }
}

public record MerchantInfo
{
    public required string Id { get; init; }
    public required string BusinessName { get; init; }
    public required string Role { get; init; }
}

public record RefreshTokenRequest
{
    public required string RefreshToken { get; init; }
}

public record ApiKeyAuthRequest
{
    public required string KeyId { get; init; }
    public required string KeySecret { get; init; }
}

public record RegisterRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Phone { get; init; }
}

public record ChangePasswordRequest
{
    public required string CurrentPassword { get; init; }
    public required string NewPassword { get; init; }
}

public record EnableMfaResponse
{
    public required string Secret { get; init; }
    public required string QrCodeUri { get; init; }
    public required string[] BackupCodes { get; init; }
}
