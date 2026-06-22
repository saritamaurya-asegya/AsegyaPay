using AsegyaPay.AuthService.Domain.Entities;
using AsegyaPay.AuthService.Application.Interfaces;
using AsegyaPay.SharedKernel.Application;
using AsegyaPay.SharedKernel.Common;
using FluentValidation;

namespace AsegyaPay.AuthService.Application.Commands.Login;

// ── Command ──────────────────────────────────────────────────────────────────

public sealed record LoginCommand(
    string Email,
    string Password,
    string? MfaToken = null,
    string? IpAddress = null,
    string? UserAgent = null
) : ICommand<Result<LoginResponse>>;

public sealed record LoginResponse(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    string TokenType = "Bearer",
    bool MfaRequired = false,
    string? MfaChallenge = null
);

// ── Validator ─────────────────────────────────────────────────────────────────

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
    }
}

// ── Handler ───────────────────────────────────────────────────────────────────

public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IMfaService _mfaService;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepository,
        IMfaService mfaService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _mfaService = mfaService;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
            return Result.Failure<LoginResponse>(
                Error.Unauthorized("Invalid credentials."));

        if (!user.IsActive)
            return Result.Failure<LoginResponse>(
                Error.Unauthorized("Account is deactivated."));

        if (user.IsLockedOut())
            return Result.Failure<LoginResponse>(
                Error.Unauthorized("Account is temporarily locked due to too many failed attempts."));

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            user.RecordFailedLogin();
            await _userRepository.UpdateAsync(user, cancellationToken);
            return Result.Failure<LoginResponse>(Error.Unauthorized("Invalid credentials."));
        }

        if (user.IsMfaEnabled)
        {
            if (string.IsNullOrEmpty(request.MfaToken))
            {
                return Result.Success(new LoginResponse(
                    AccessToken: string.Empty,
                    RefreshToken: string.Empty,
                    ExpiresIn: 0,
                    MfaRequired: true,
                    MfaChallenge: "totp"));
            }

            if (!_mfaService.ValidateTotp(user.MfaSecret!, request.MfaToken))
                return Result.Failure<LoginResponse>(
                    Error.Unauthorized("Invalid MFA token."));
        }

        user.RecordSuccessfulLogin();
        await _userRepository.UpdateAsync(user, cancellationToken);

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken(
            Id: Guid.NewGuid(),
            UserId: user.Id,
            Token: refreshToken,
            ExpiresAt: DateTime.UtcNow.AddDays(30),
            IsRevoked: false,
            CreatedAt: DateTime.UtcNow);

        await _refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);

        return Result.Success(new LoginResponse(
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            ExpiresIn: 3600));
    }
}
