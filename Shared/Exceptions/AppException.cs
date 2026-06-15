namespace Travel.Shared.Exceptions;

public class AppException : Exception
{
    public int StatusCode { get; set; }
    public string ErrorCode { get; set; }

    public AppException(int statusCode, string errorMessageWithCode) 
        : base(ParseMessage(errorMessageWithCode))
    {
        StatusCode = statusCode;
        ErrorCode = ParseErrorCode(errorMessageWithCode);
    }

    // Hàm phụ tách chữ trước dấu '|' làm ErrorCode
    private static string ParseErrorCode(string input) => input.Contains('|') ? input.Split('|')[0] : "SYSTEM_ERROR";

    // Hàm phụ tách chữ sau dấu '|' làm Message hiển thị
    private static string ParseMessage(string input) => input.Contains('|') ? input.Split('|')[1] : input;
}