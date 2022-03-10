using System.Runtime.Serialization;

namespace Saas.Admin.Service.Exceptions
{
    public class ItemNotFoundExcepton : Exception
    {
        ItemNotFoundExcepton() : base()
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
}
