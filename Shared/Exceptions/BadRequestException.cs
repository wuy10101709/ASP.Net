namespace Travel.Shared.Exceptions;

public class BadRequestException : AppException
{
    public BadRequestException(string errorMessageWithCode) : base(400, errorMessageWithCode)
    {
    }
}