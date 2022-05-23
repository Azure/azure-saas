namespace Saas.Permissions.Service.Exceptions;

public class UserNotFoundException : Exception
{
    private UserNotFoundException() : base()
    {

    }
    public UserNotFoundException(string? message) : base(message)
    {
    }

    public UserNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected UserNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
