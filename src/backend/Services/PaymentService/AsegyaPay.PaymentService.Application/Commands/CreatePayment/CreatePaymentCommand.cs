using AsegyaPay.PaymentService.Domain.Entities;
using AsegyaPay.PaymentService.Domain.Enums;
using AsegyaPay.PaymentService.Domain.ValueObjects;
using AsegyaPay.PaymentService.Application.Interfaces;
using AsegyaPay.SharedKernel.Application;
using AsegyaPay.SharedKernel.Common;
using FluentValidation;
using MediatR;

namespace AsegyaPay.PaymentService.Application.Commands.CreatePayment;

// ── Command ──────────────────────────────────────────────────────────────────

public sealed record CreatePaymentCommand(
    string MerchantId,
    string OrderId,
    decimal Amount,
    string Currency,
    PaymentMethod Method,
    string Description,
    string? CustomerId,
    string? CallbackUrl,
    string? RedirectUrl,
    Dictionary<string, string>? Metadata
) : ICommand<Result<CreatePaymentResponse>>;

public sealed record CreatePaymentResponse(
    Guid PaymentId,
    string OrderId,
    decimal Amount,
    string Currency,
    PaymentStatus Status,
    string? PaymentUrl,
    DateTime ExpiresAt
);

// ── Validator ─────────────────────────────────────────────────────────────────

public sealed class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
{
    public CreatePaymentCommandValidator()
    {
        RuleFor(x => x.MerchantId)
            .NotEmpty().WithMessage("MerchantId is required.");

        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("OrderId is required.")
            .MaximumLength(64).WithMessage("OrderId cannot exceed 64 characters.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0.")
            .LessThanOrEqualTo(10_000_000).WithMessage("Amount cannot exceed 10,000,000.");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3).WithMessage("Currency must be a 3-letter ISO code.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(255);
    }
}

// ── Handler ───────────────────────────────────────────────────────────────────

public sealed class CreatePaymentCommandHandler
    : ICommandHandler<CreatePaymentCommand, Result<CreatePaymentResponse>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentGatewayProvider _gatewayProvider;

    public CreatePaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IPaymentGatewayProvider gatewayProvider)
    {
        _paymentRepository = paymentRepository;
        _gatewayProvider = gatewayProvider;
    }

    public async Task<Result<CreatePaymentResponse>> Handle(
        CreatePaymentCommand request,
        CancellationToken cancellationToken)
    {
        var money = Money.Of(request.Amount, request.Currency);

        var payment = Payment.Create(
            orderId: request.OrderId,
            merchantId: request.MerchantId,
            amount: money,
            method: request.Method,
            description: request.Description,
            customerId: request.CustomerId,
            callbackUrl: request.CallbackUrl,
            redirectUrl: request.RedirectUrl,
            metadata: request.Metadata);

        var gatewayResult = await _gatewayProvider.CreateOrderAsync(
            request.MerchantId,
            money,
            request.Currency,
            request.Metadata ?? []);

        if (!gatewayResult.IsSuccess)
            return Result.Failure<CreatePaymentResponse>(
                Error.Validation("Payment.GatewayError", gatewayResult.Error ?? "Gateway failed to create order."));

        await _paymentRepository.AddAsync(payment, cancellationToken);

        return Result.Success(new CreatePaymentResponse(
            PaymentId: payment.Id,
            OrderId: payment.OrderId,
            Amount: payment.Amount.Amount,
            Currency: payment.Amount.Currency,
            Status: payment.Status,
            PaymentUrl: $"/pay/{payment.Id}",
            ExpiresAt: payment.ExpiresAt!.Value));
    }
}
