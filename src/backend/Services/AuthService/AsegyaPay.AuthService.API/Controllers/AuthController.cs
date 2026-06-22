using AsegyaPay.AuthService.Application.Commands.Login;
using AsegyaPay.AuthService.Application.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AsegyaPay.AuthService.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
[Produces("application/json")]
public sealed class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Authenticate and obtain access + refresh tokens.
    /// </summary>
    [HttpPost("token")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = Request.Headers.UserAgent.ToString();

        var command = new LoginCommand(
            Email: request.Email,
            Password: request.Password,
            MfaToken: request.MfaToken,
            IpAddress: ipAddress,
            UserAgent: userAgent);

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
            return Unauthorized(new { error = result.Error.Code, message = result.Error.Description });

        return Ok(result.Value);
    }

    /// <summary>
    /// Register a new user account.
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterCommand(
            Email: request.Email,
            Password: request.Password,
            FirstName: request.FirstName,
            LastName: request.LastName,
            PhoneNumber: request.PhoneNumber);

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error.Code.Contains("Duplicate"))
                return Conflict(new { error = result.Error.Code, message = result.Error.Description });
            return BadRequest(new { error = result.Error.Code, message = result.Error.Description });
        }

        return StatusCode(StatusCodes.Status201Created, result.Value);
    }

    /// <summary>
    /// Health check endpoint.
    /// </summary>
    [HttpGet("health")]
    public IActionResult Health() => Ok(new { status = "healthy", service = "auth-service" });
}

public sealed record LoginRequest(string Email, string Password, string? MfaToken = null);
public sealed record RegisterRequest(string Email, string Password, string FirstName, string LastName, string? PhoneNumber = null);
