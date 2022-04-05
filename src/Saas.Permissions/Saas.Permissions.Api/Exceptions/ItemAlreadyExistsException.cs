namespace Saas.Permissions.Api.Exceptions;

public class ItemAlreadyExistsException : Exception
{
    private ItemAlreadyExistsException() : base()
    {

    }
    public ItemAlreadyExistsException(string? message) : base(message)
    {
    }

    public ItemAlreadyExistsException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected ItemAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
