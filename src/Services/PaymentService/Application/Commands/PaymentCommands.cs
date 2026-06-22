using AsegyaPay.Common.Models;
using AsegyaPay.Contracts.Payments;
using MediatR;

namespace PaymentService.Application.Commands;

public record CreatePaymentCommand : IRequest<ApiResponse<PaymentResponse>>
{
    public required string MerchantId { get; init; }
    public required decimal Amount { get; init; }
    public required string Currency { get; init; }
    public required string Method { get; init; }
    public string? Description { get; init; }
    public string? OrderId { get; init; }
    public string? CustomerId { get; init; }
    public string? ReturnUrl { get; init; }
    public string? CallbackUrl { get; init; }
    public Dictionary<string, string>? Metadata { get; init; }
}

public record CapturePaymentCommand : IRequest<ApiResponse<PaymentResponse>>
{
    public required Guid PaymentId { get; init; }
    public decimal? Amount { get; init; }
}

public record RefundPaymentCommand : IRequest<ApiResponse<RefundResponse>>
{
    public required Guid PaymentId { get; init; }
    public required decimal Amount { get; init; }
    public string? Reason { get; init; }
}

public record GetPaymentQuery : IRequest<ApiResponse<PaymentResponse>>
{
    public required Guid PaymentId { get; init; }
    public required string MerchantId { get; init; }
}

public record ListPaymentsQuery : IRequest<ApiResponse<PaginatedResponse<PaymentResponse>>>
{
    public required string MerchantId { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Status { get; init; }
    public string? Method { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
}
