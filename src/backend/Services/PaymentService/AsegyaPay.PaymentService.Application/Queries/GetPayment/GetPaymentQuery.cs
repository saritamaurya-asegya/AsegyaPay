using AsegyaPay.PaymentService.Domain.Entities;
using AsegyaPay.PaymentService.Domain.Enums;
using AsegyaPay.PaymentService.Application.Interfaces;
using AsegyaPay.SharedKernel.Application;
using AsegyaPay.SharedKernel.Common;

namespace AsegyaPay.PaymentService.Application.Queries.GetPayment;

// ── Query ─────────────────────────────────────────────────────────────────────

public sealed record GetPaymentQuery(Guid PaymentId, string MerchantId)
    : IQuery<Result<PaymentDto>>;

// ── DTO ───────────────────────────────────────────────────────────────────────

public sealed record PaymentDto(
    Guid Id,
    string OrderId,
    string MerchantId,
    string? CustomerId,
    decimal Amount,
    string Currency,
    PaymentStatus Status,
    PaymentMethod Method,
    string Description,
    string? GatewayTransactionId,
    int FraudScore,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime? CapturedAt,
    IEnumerable<RefundDto> Refunds
);

public sealed record RefundDto(
    Guid Id,
    decimal Amount,
    string Currency,
    string Reason,
    RefundStatus Status,
    DateTime CreatedAt
);

// ── Handler ───────────────────────────────────────────────────────────────────

public sealed class GetPaymentQueryHandler : IQueryHandler<GetPaymentQuery, Result<PaymentDto>>
{
    private readonly IPaymentRepository _paymentRepository;

    public GetPaymentQueryHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<Result<PaymentDto>> Handle(GetPaymentQuery request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);

        if (payment is null)
            return Result.Failure<PaymentDto>(Error.NotFound(nameof(Payment), request.PaymentId));

        if (payment.MerchantId != request.MerchantId)
            return Result.Failure<PaymentDto>(Error.Forbidden());

        return Result.Success(MapToDto(payment));
    }

    private static PaymentDto MapToDto(Payment payment) => new(
        Id: payment.Id,
        OrderId: payment.OrderId,
        MerchantId: payment.MerchantId,
        CustomerId: payment.CustomerId,
        Amount: payment.Amount.Amount,
        Currency: payment.Amount.Currency,
        Status: payment.Status,
        Method: payment.Method,
        Description: payment.Description,
        GatewayTransactionId: payment.GatewayTransactionId,
        FraudScore: payment.FraudScore,
        CreatedAt: payment.CreatedAt,
        UpdatedAt: payment.UpdatedAt,
        CapturedAt: payment.CapturedAt,
        Refunds: payment.Refunds.Select(r => new RefundDto(
            Id: r.Id,
            Amount: r.Amount.Amount,
            Currency: r.Amount.Currency,
            Reason: r.Reason,
            Status: r.Status,
            CreatedAt: r.CreatedAt)));
}
