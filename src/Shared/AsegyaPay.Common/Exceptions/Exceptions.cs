namespace AsegyaPay.Common.Exceptions;

/// <summary>
/// Base exception for all AsegyaPay domain exceptions.
/// </summary>
public abstract class AsegyaPayException : Exception
{
    public string ErrorCode { get; }
    public int HttpStatusCode { get; }

    protected AsegyaPayException(string message, string errorCode, int httpStatusCode = 400)
        : base(message)
    {
        ErrorCode = errorCode;
        HttpStatusCode = httpStatusCode;
    }
}

public class NotFoundException : AsegyaPayException
{
    public NotFoundException(string entity, object id)
        : base($"{entity} with id '{id}' was not found.", "NOT_FOUND", 404) { }
}

public class ValidationException : AsegyaPayException
{
    public List<string> ValidationErrors { get; }

    public ValidationException(List<string> errors)
        : base("One or more validation errors occurred.", "VALIDATION_ERROR", 422)
    {
        ValidationErrors = errors;
    }
}

public class UnauthorizedException : AsegyaPayException
{
    public UnauthorizedException(string message = "Unauthorized access.")
        : base(message, "UNAUTHORIZED", 401) { }
}

public class ForbiddenException : AsegyaPayException
{
    public ForbiddenException(string message = "Access forbidden.")
        : base(message, "FORBIDDEN", 403) { }
}

public class ConflictException : AsegyaPayException
{
    public ConflictException(string message)
        : base(message, "CONFLICT", 409) { }
}

public class PaymentProcessingException : AsegyaPayException
{
    public PaymentProcessingException(string message)
        : base(message, "PAYMENT_PROCESSING_ERROR", 502) { }
}

public class RateLimitException : AsegyaPayException
{
    public RateLimitException()
        : base("Rate limit exceeded. Please try again later.", "RATE_LIMIT_EXCEEDED", 429) { }
}

public class FraudDetectedException : AsegyaPayException
{
    public string RiskScore { get; }

    public FraudDetectedException(string message, string riskScore)
        : base(message, "FRAUD_DETECTED", 403)
    {
        RiskScore = riskScore;
    }
}
