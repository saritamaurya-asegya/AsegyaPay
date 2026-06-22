namespace AsegyaPay.Common.Models;

/// <summary>
/// Standard API response wrapper for consistent API responses.
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = new();
    public ApiMetadata? Metadata { get; set; }

    public static ApiResponse<T> Ok(T data, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    public static ApiResponse<T> Fail(string message, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors ?? new()
        };
    }
}

public class ApiMetadata
{
    public int? Page { get; set; }
    public int? PageSize { get; set; }
    public long? TotalCount { get; set; }
    public int? TotalPages { get; set; }
    public string? RequestId { get; set; }
    public long? ProcessingTimeMs { get; set; }
}

/// <summary>
/// Paginated response for list endpoints.
/// </summary>
public class PaginatedResponse<T>
{
    public List<T> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public long TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}
