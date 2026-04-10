namespace LMS.Application.Common;

public class BaseResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public static BaseResponse<T> Ok(T data, string message = "Success")
    {
        return new BaseResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static BaseResponse<T> Fail(string message)
    {
        return new BaseResponse<T>
        {
            Success = false,
            Message = message,
            Data = default
        };
    }
}
