using AsegyaPay.PaymentService.Domain.Entities;
using AsegyaPay.PaymentService.Domain.ValueObjects;
using AsegyaPay.PaymentService.Application.Interfaces;
using AsegyaPay.SharedKernel.Application;
using AsegyaPay.SharedKernel.Common;
using FluentValidation;

namespace AsegyaPay.PaymentService.Application.Commands.CapturePayment;

// ── Command ──────────────────────────────────────────────────────────────────

public sealed record CapturePaymentCommand(
    Guid PaymentId,
    string MerchantId,
    decimal? CaptureAmount = null
) : ICommand<Result<CapturePaymentResponse>>;

public sealed record CapturePaymentResponse(
    Guid PaymentId,
    decimal CapturedAmount,
    string Currency,
    string Status
);

// ── Validator ─────────────────────────────────────────────────────────────────

public sealed class CapturePaymentCommandValidator : AbstractValidator<CapturePaymentCommand>
{
    public CapturePaymentCommandValidator()
    {
        RuleFor(x => x.PaymentId).NotEmpty();
        RuleFor(x => x.MerchantId).NotEmpty();
        RuleFor(x => x.CaptureAmount)
            .GreaterThan(0).When(x => x.CaptureAmount.HasValue)
            .WithMessage("Capture amount must be greater than 0.");
    }
}

// ── Handler ───────────────────────────────────────────────────────────────────

public sealed class CapturePaymentCommandHandler
    : ICommandHandler<CapturePaymentCommand, Result<CapturePaymentResponse>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentGatewayProvider _gatewayProvider;

    public CapturePaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IPaymentGatewayProvider gatewayProvider)
    {
        _paymentRepository = paymentRepository;
        _gatewayProvider = gatewayProvider;
    }

    public async Task<Result<CapturePaymentResponse>> Handle(
        CapturePaymentCommand request,
        CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);

        if (payment is null)
            return Result.Failure<CapturePaymentResponse>(
                Error.NotFound(nameof(Payment), request.PaymentId));

        if (payment.MerchantId != request.MerchantId)
            return Result.Failure<CapturePaymentResponse>(Error.Forbidden());

        Money? captureAmount = request.CaptureAmount.HasValue
            ? Money.Of(request.CaptureAmount.Value, payment.Amount.Currency)
            : null;

        var gatewayResult = await _gatewayProvider.CaptureAsync(
            payment.GatewayTransactionId!,
            captureAmount ?? payment.Amount);

        if (!gatewayResult.IsSuccess)
            return Result.Failure<CapturePaymentResponse>(
                Error.Validation("Payment.CaptureError", gatewayResult.Error ?? "Capture failed."));

        payment.Capture(captureAmount);
        await _paymentRepository.UpdateAsync(payment, cancellationToken);

        return Result.Success(new CapturePaymentResponse(
            PaymentId: payment.Id,
            CapturedAmount: payment.CapturedAmount!.Amount,
            Currency: payment.CapturedAmount!.Currency,
            Status: payment.Status.ToString()));
    }
}
