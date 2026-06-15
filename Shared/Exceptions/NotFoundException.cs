namespace Travel.Shared.Exceptions;

public class NotFoundException : AppException
{
    public NotFoundException(string errorMessageWithCode) : base(404, errorMessageWithCode)
    {
    }
}