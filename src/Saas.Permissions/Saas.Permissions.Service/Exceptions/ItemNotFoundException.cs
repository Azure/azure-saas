namespace Saas.Permissions.Service.Exceptions;

public class ItemNotFoundExcepton : Exception
{
    private ItemNotFoundExcepton() : base()
    {

    }
    public ItemNotFoundExcepton(string? message) : base(message)
    {
    }

    public ItemNotFoundExcepton(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected ItemNotFoundExcepton(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
