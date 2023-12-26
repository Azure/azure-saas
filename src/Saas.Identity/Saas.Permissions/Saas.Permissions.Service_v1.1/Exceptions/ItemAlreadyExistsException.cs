namespace Saas.Permissions.Service.Exceptions;

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
}
