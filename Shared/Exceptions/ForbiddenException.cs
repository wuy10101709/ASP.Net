namespace Travel.Shared.Exceptions;

public class ForbiddenException : AppException
{
    public ForbiddenException(string errorMessageWithCode) : base(403, errorMessageWithCode)
    {
    }
}