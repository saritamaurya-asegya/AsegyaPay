using AsegyaPay.Common.Models;
using AsegyaPay.Contracts.Payments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Application.Commands;

namespace PaymentService.API.Controllers;

/// <summary>
/// Payment API endpoints for creating, capturing, and managing payments.
/// </summary>
[ApiController]
[Route("api/v1/payments")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(IMediator mediator, ILogger<PaymentsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Create a new payment order.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PaymentResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<PaymentResponse>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePayment(
        [FromBody] CreatePaymentRequest request,
        CancellationToken cancellationToken)
    {
        var merchantId = GetMerchantId();

        var command = new CreatePaymentCommand
        {
            MerchantId = merchantId,
            Amount = request.Amount,
            Currency = request.Currency,
            Method = request.Method,
            Description = request.Description,
            OrderId = request.OrderId,
            CustomerId = request.CustomerId,
            ReturnUrl = request.ReturnUrl,
            CallbackUrl = request.CallbackUrl,
            Metadata = request.Metadata
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetPayment), new { id = result.Data!.Id }, result);
    }

    /// <summary>
    /// Retrieve payment details by ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<PaymentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPayment(string id, CancellationToken cancellationToken)
    {
        var query = new GetPaymentQuery
        {
            PaymentId = Guid.Parse(id),
            MerchantId = GetMerchantId()
        };

        var result = await _mediator.Send(query, cancellationToken);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// List payments with filtering and pagination.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<PaymentResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListPayments(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        [FromQuery] string? method = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        CancellationToken cancellationToken = default)
    {
        var query = new ListPaymentsQuery
        {
            MerchantId = GetMerchantId(),
            Page = page,
            PageSize = Math.Min(pageSize, 100),
            Status = status,
            Method = method,
            FromDate = from,
            ToDate = to
        };

        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Capture an authorized payment.
    /// </summary>
    [HttpPost("{id}/capture")]
    [ProducesResponseType(typeof(ApiResponse<PaymentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CapturePayment(
        string id,
        [FromBody] CapturePaymentRequest? request,
        CancellationToken cancellationToken)
    {
        var command = new CapturePaymentCommand
        {
            PaymentId = Guid.Parse(id),
            Amount = request?.Amount
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Refund a captured payment (full or partial).
    /// </summary>
    [HttpPost("{id}/refund")]
    [ProducesResponseType(typeof(ApiResponse<RefundResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefundPayment(
        string id,
        [FromBody] RefundRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RefundPaymentCommand
        {
            PaymentId = Guid.Parse(id),
            Amount = request.Amount,
            Reason = request.Reason
        };

        var result = await _mediator.Send(command, cancellationToken);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    private string GetMerchantId()
    {
        return User.FindFirst("merchant_id")?.Value
            ?? throw new UnauthorizedAccessException("Merchant context not found.");
    }
}
