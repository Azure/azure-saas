namespace Saas.Domain.Exceptions
{
    public class TenantNotFoundException : Exception
    {
        public TenantNotFoundException()
        {
        }

        public TenantNotFoundException(string message)
            : base(message)
        {
        }

        public TenantNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
