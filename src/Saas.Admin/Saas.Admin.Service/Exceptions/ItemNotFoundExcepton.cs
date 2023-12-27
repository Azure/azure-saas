namespace Saas.Admin.Service.Exceptions;

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
}
