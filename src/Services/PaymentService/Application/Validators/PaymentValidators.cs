using FluentValidation;
using PaymentService.Application.Commands;

namespace PaymentService.Application.Validators;

public class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
{
    private static readonly string[] SupportedCurrencies = { "INR", "USD", "EUR", "GBP", "SGD", "AED" };
    private static readonly string[] SupportedMethods = { "card", "upi", "netbanking", "wallet", "emi", "bnpl" };

    public CreatePaymentCommandValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than zero.")
            .LessThanOrEqualTo(10_000_000).WithMessage("Amount exceeds maximum limit.");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required.")
            .Must(c => SupportedCurrencies.Contains(c.ToUpperInvariant()))
            .WithMessage("Unsupported currency.");

        RuleFor(x => x.Method)
            .NotEmpty().WithMessage("Payment method is required.")
            .Must(m => SupportedMethods.Contains(m.ToLowerInvariant()))
            .WithMessage("Unsupported payment method.");

        RuleFor(x => x.MerchantId)
            .NotEmpty().WithMessage("Merchant ID is required.");
    }
}

public class RefundPaymentCommandValidator : AbstractValidator<RefundPaymentCommand>
{
    public RefundPaymentCommandValidator()
    {
        RuleFor(x => x.PaymentId)
            .NotEmpty().WithMessage("Payment ID is required.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Refund amount must be greater than zero.");
    }
}
