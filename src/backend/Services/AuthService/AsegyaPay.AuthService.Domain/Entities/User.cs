using AsegyaPay.SharedKernel.Domain;

namespace AsegyaPay.AuthService.Domain.Entities;

/// <summary>
/// User aggregate root — represents a platform user (merchant admin, staff, or super-admin).
/// </summary>
public sealed class User : AggregateRoot<Guid>
{
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string? PhoneNumber { get; private set; }
    public UserRole Role { get; private set; }
    public string? MerchantId { get; private set; }
    public bool IsEmailVerified { get; private set; }
    public bool IsPhoneVerified { get; private set; }
    public bool IsMfaEnabled { get; private set; }
    public string? MfaSecret { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsLocked { get; private set; }
    public int FailedLoginAttempts { get; private set; }
    public DateTime? LockedUntil { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    private User() : base(Guid.NewGuid()) { }

    public static User Create(
        string email,
        string passwordHash,
        string firstName,
        string lastName,
        UserRole role,
        string? merchantId = null,
        string? phoneNumber = null)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email.ToLowerInvariant().Trim(),
            PasswordHash = passwordHash,
            FirstName = firstName,
            LastName = lastName,
            Role = role,
            MerchantId = merchantId,
            PhoneNumber = phoneNumber,
            IsEmailVerified = false,
            IsPhoneVerified = false,
            IsMfaEnabled = false,
            IsActive = true,
            IsLocked = false,
            FailedLoginAttempts = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void RecordSuccessfulLogin()
    {
        FailedLoginAttempts = 0;
        IsLocked = false;
        LockedUntil = null;
        LastLoginAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordFailedLogin(int maxAttempts = 5, int lockoutMinutes = 30)
    {
        FailedLoginAttempts++;
        if (FailedLoginAttempts >= maxAttempts)
        {
            IsLocked = true;
            LockedUntil = DateTime.UtcNow.AddMinutes(lockoutMinutes);
        }
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsLockedOut() => IsLocked && (LockedUntil == null || LockedUntil > DateTime.UtcNow);

    public void VerifyEmail()
    {
        IsEmailVerified = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void EnableMfa(string mfaSecret)
    {
        MfaSecret = mfaSecret;
        IsMfaEnabled = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePasswordHash(string newHash)
    {
        PasswordHash = newHash;
        UpdatedAt = DateTime.UtcNow;
    }
}

public enum UserRole
{
    MerchantOwner = 0,
    MerchantAdmin = 1,
    MerchantStaff = 2,
    SuperAdmin = 3,
    SupportAgent = 4,
    ComplianceOfficer = 5
}
