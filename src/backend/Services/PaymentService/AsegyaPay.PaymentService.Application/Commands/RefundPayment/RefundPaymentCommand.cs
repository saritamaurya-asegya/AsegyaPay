using AsegyaPay.PaymentService.Domain.Entities;
using AsegyaPay.PaymentService.Domain.ValueObjects;
using AsegyaPay.PaymentService.Application.Interfaces;
using AsegyaPay.SharedKernel.Application;
using AsegyaPay.SharedKernel.Common;
using FluentValidation;

namespace AsegyaPay.PaymentService.Application.Commands.RefundPayment;

// ── Command ──────────────────────────────────────────────────────────────────

public sealed record RefundPaymentCommand(
    Guid PaymentId,
    string MerchantId,
    decimal Amount,
    string Reason,
    string InitiatedBy
) : ICommand<Result<RefundPaymentResponse>>;

public sealed record RefundPaymentResponse(
    Guid RefundId,
    Guid PaymentId,
    decimal RefundAmount,
    string Currency,
    string Status
);

// ── Validator ─────────────────────────────────────────────────────────────────

public sealed class RefundPaymentCommandValidator : AbstractValidator<RefundPaymentCommand>
{
    public RefundPaymentCommandValidator()
    {
        RuleFor(x => x.PaymentId).NotEmpty();
        RuleFor(x => x.MerchantId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(255);
        RuleFor(x => x.InitiatedBy).NotEmpty();
    }
}

// ── Handler ───────────────────────────────────────────────────────────────────

public sealed class RefundPaymentCommandHandler
    : ICommandHandler<RefundPaymentCommand, Result<RefundPaymentResponse>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentGatewayProvider _gatewayProvider;

    public RefundPaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IPaymentGatewayProvider gatewayProvider)
    {
        _paymentRepository = paymentRepository;
        _gatewayProvider = gatewayProvider;
    }

    public async Task<Result<RefundPaymentResponse>> Handle(
        RefundPaymentCommand request,
        CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);

        if (payment is null)
            return Result.Failure<RefundPaymentResponse>(
                Error.NotFound(nameof(Payment), request.PaymentId));

        if (payment.MerchantId != request.MerchantId)
            return Result.Failure<RefundPaymentResponse>(Error.Forbidden());

        var refundMoney = Money.Of(request.Amount, payment.Amount.Currency);

        Refund refund;
        try
        {
            refund = payment.Refund(refundMoney, request.Reason, request.InitiatedBy);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<RefundPaymentResponse>(
                Error.Validation("Payment.RefundError", ex.Message));
        }

        var gatewayResult = await _gatewayProvider.RefundAsync(
            payment.GatewayTransactionId!,
            refundMoney,
            request.Reason);

        if (gatewayResult.IsSuccess)
            refund.MarkProcessed(gatewayResult.RefundId!);
        else
            refund.MarkFailed(gatewayResult.Error ?? "Gateway refund failed.");

        await _paymentRepository.UpdateAsync(payment, cancellationToken);

        return Result.Success(new RefundPaymentResponse(
            RefundId: refund.Id,
            PaymentId: payment.Id,
            RefundAmount: refund.Amount.Amount,
            Currency: refund.Amount.Currency,
            Status: refund.Status.ToString()));
    }
}
