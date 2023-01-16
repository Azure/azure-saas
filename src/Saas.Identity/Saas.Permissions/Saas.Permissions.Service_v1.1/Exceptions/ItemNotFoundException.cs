namespace Saas.Permissions.Service.Exceptions;

public class ItemNotFoundException : Exception
{
    private ItemNotFoundException() : base()
    {

    }
    public ItemNotFoundException(string? message) : base(message)
    {
    }

    public ItemNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected ItemNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
