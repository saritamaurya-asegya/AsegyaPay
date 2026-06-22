using AsegyaPay.AuthService.Domain.Entities;
using AsegyaPay.AuthService.Application.Interfaces;
using AsegyaPay.SharedKernel.Application;
using AsegyaPay.SharedKernel.Common;
using FluentValidation;

namespace AsegyaPay.AuthService.Application.Commands.Register;

public sealed record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    UserRole Role = UserRole.MerchantOwner,
    string? MerchantId = null
) : ICommand<Result<RegisterResponse>>;

public sealed record RegisterResponse(Guid UserId, string Email, bool EmailVerificationRequired);

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one number.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(64);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(64);
    }
}

public sealed class RegisterCommandHandler : ICommandHandler<RegisterCommand, Result<RegisterResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<RegisterResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
            return Result.Failure<RegisterResponse>(
                Error.Conflict("User.DuplicateEmail", $"A user with email '{request.Email}' already exists."));

        var passwordHash = _passwordHasher.Hash(request.Password);

        var user = User.Create(
            email: request.Email,
            passwordHash: passwordHash,
            firstName: request.FirstName,
            lastName: request.LastName,
            role: request.Role,
            merchantId: request.MerchantId,
            phoneNumber: request.PhoneNumber);

        await _userRepository.AddAsync(user, cancellationToken);

        return Result.Success(new RegisterResponse(
            UserId: user.Id,
            Email: user.Email,
            EmailVerificationRequired: true));
    }
}
