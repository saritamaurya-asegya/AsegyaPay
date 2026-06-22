using AsegyaPay.PaymentService.Domain.Enums;
using AsegyaPay.PaymentService.Domain.ValueObjects;

namespace AsegyaPay.PaymentService.Application.Interfaces;

/// <summary>
/// Repository interface for Payment aggregate.
/// Follows the repository pattern — infrastructure concern, domain interface.
/// </summary>
public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Payment?> GetByOrderIdAsync(string orderId, string merchantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Payment>> GetByMerchantIdAsync(string merchantId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountByMerchantIdAsync(string merchantId, CancellationToken cancellationToken = default);
    Task AddAsync(Payment payment, CancellationToken cancellationToken = default);
    Task UpdateAsync(Payment payment, CancellationToken cancellationToken = default);
}

/// <summary>
/// Payment gateway provider interface for interacting with external payment processors.
/// </summary>
public interface IPaymentGatewayProvider
{
    Task<GatewayOrderResult> CreateOrderAsync(string merchantId, Money amount, string currency, Dictionary<string, string> metadata);
    Task<GatewayAuthorizationResult> AuthorizeAsync(string gatewayOrderId, string paymentToken);
    Task<GatewayCaptureResult> CaptureAsync(string gatewayTransactionId, Money amount);
    Task<GatewayRefundResult> RefundAsync(string gatewayTransactionId, Money refundAmount, string reason);
    bool VerifySignature(string payload, string signature, string secret);
}

public record GatewayOrderResult(bool IsSuccess, string? OrderId, string? Error);
public record GatewayAuthorizationResult(bool IsSuccess, string? TransactionId, string? Error);
public record GatewayCaptureResult(bool IsSuccess, string? CaptureId, string? Error);
public record GatewayRefundResult(bool IsSuccess, string? RefundId, string? Error);
