namespace Backend.Application.Dtos.Api;

// Every error response uses this shape
public class ApiResponse<T>
{
    public ApiResponse(int status, string message, T? data = default)
    {
        Status = status;
        Message = message;
        Data = data;
    }
    public bool Success => true;
    public int Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
}