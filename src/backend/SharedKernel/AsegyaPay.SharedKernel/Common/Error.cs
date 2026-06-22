namespace AsegyaPay.SharedKernel.Common;

/// <summary>
/// Represents a domain error with a code and description.
/// </summary>
public sealed record Error(string Code, string Description)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("Error.NullValue", "The specified result value is null.");

    public static Error NotFound(string entity, object id) =>
        new($"{entity}.NotFound", $"{entity} with id '{id}' was not found.");

    public static Error Validation(string code, string description) =>
        new(code, description);

    public static Error Conflict(string code, string description) =>
        new(code, description);

    public static Error Unauthorized(string description = "Unauthorized access.") =>
        new("Error.Unauthorized", description);

    public static Error Forbidden(string description = "Access denied.") =>
        new("Error.Forbidden", description);
}
