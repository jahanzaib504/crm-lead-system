namespace Backend.Application.Dtos;

// Every error response uses this shape
public class ApiError
{
    public bool Success => false;
    public int Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public Dictionary<string, string[]>? Errors { get; set; }
    public string? TraceId { get; set; }
}