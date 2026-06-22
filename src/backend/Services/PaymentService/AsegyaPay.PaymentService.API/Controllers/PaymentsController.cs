using AsegyaPay.PaymentService.Application.Commands.CreatePayment;
using AsegyaPay.PaymentService.Application.Commands.CapturePayment;
using AsegyaPay.PaymentService.Application.Commands.RefundPayment;
using AsegyaPay.PaymentService.Application.Queries.GetPayment;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AsegyaPay.PaymentService.API.Controllers;

[ApiController]
[Route("api/v1/payments")]
[Produces("application/json")]
public sealed class PaymentsController : ControllerBase
{
    private readonly ISender _sender;

    public PaymentsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Create a new payment order.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreatePaymentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePayment(
        [FromBody] CreatePaymentRequest request,
        CancellationToken cancellationToken)
    {
        var merchantId = User.FindFirst("merchant_id")?.Value ?? string.Empty;

        var command = new CreatePaymentCommand(
            MerchantId: merchantId,
            OrderId: request.OrderId,
            Amount: request.Amount,
            Currency: request.Currency,
            Method: request.Method,
            Description: request.Description,
            CustomerId: request.CustomerId,
            CallbackUrl: request.CallbackUrl,
            RedirectUrl: request.RedirectUrl,
            Metadata: request.Metadata);

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error.Code, message = result.Error.Description });

        return CreatedAtAction(nameof(GetPayment), new { id = result.Value.PaymentId }, result.Value);
    }

    /// <summary>
    /// Get payment details by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPayment(Guid id, CancellationToken cancellationToken)
    {
        var merchantId = User.FindFirst("merchant_id")?.Value ?? string.Empty;
        var query = new GetPaymentQuery(id, merchantId);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
            return result.Error.Code.Contains("NotFound") ? NotFound() : Forbid();

        return Ok(result.Value);
    }

    /// <summary>
    /// Capture an authorized payment.
    /// </summary>
    [HttpPost("{id:guid}/capture")]
    [ProducesResponseType(typeof(CapturePaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CapturePayment(
        Guid id,
        [FromBody] CapturePaymentRequest request,
        CancellationToken cancellationToken)
    {
        var merchantId = User.FindFirst("merchant_id")?.Value ?? string.Empty;
        var command = new CapturePaymentCommand(id, merchantId, request.Amount);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.Code.Contains("NotFound")
                ? NotFound()
                : BadRequest(new { error = result.Error.Code, message = result.Error.Description });

        return Ok(result.Value);
    }

    /// <summary>
    /// Refund a captured payment (full or partial).
    /// </summary>
    [HttpPost("{id:guid}/refund")]
    [ProducesResponseType(typeof(RefundPaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RefundPayment(
        Guid id,
        [FromBody] RefundPaymentRequest request,
        CancellationToken cancellationToken)
    {
        var merchantId = User.FindFirst("merchant_id")?.Value ?? string.Empty;
        var initiatedBy = User.FindFirst("sub")?.Value ?? "system";

        var command = new RefundPaymentCommand(id, merchantId, request.Amount, request.Reason, initiatedBy);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return result.Error.Code.Contains("NotFound")
                ? NotFound()
                : BadRequest(new { error = result.Error.Code, message = result.Error.Description });

        return Ok(result.Value);
    }
}

// ── Request Models ─────────────────────────────────────────────────────────────

public sealed record CreatePaymentRequest(
    string OrderId,
    decimal Amount,
    string Currency,
    AsegyaPay.PaymentService.Domain.Enums.PaymentMethod Method,
    string Description,
    string? CustomerId,
    string? CallbackUrl,
    string? RedirectUrl,
    Dictionary<string, string>? Metadata
);

public sealed record CapturePaymentRequest(decimal? Amount);

public sealed record RefundPaymentRequest(decimal Amount, string Reason);
