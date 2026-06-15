using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Travel.Shared.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Xảy ra ngoại lệ hệ thống: {Message}", exception.Message);

        int statusCode = StatusCodes.Status500InternalServerError;
        string errorCode = "INTERNAL_SERVER_ERROR";
        string message = "Đã xảy ra lỗi hệ thống nghiêm trọng. Vui lòng thử lại sau.";

        // Nếu lỗi do chính chúng ta chủ động ném ra (AppException)
        if (exception is AppException appException)
        {
            statusCode = appException.StatusCode;
            errorCode = appException.ErrorCode;
            message = appException.Message;
        }

        // Tạo cấu trúc JSON phản hồi chuẩn hóa theo mô hình RFC 7807 (ProblemDetails)
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = errorCode,
            Detail = message,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = statusCode;

        // Ghi dữ liệu JSON trả về cho client
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        // Trả về true để báo với ASP.NET Core là lỗi đã được xử lý xong, không cần sập ứng dụng
        return true; 
    }
}