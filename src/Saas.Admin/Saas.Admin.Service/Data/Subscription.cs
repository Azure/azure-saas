using Dawn;


using System.ComponentModel.DataAnnotations;
using System.Runtime.Versioning;

namespace Saas.Admin.Service.Data
{
    public class Subscription
    {
        public Subscription(string name, string userId, byte[] concurrencyToken)
        {
            Name = Guard.Argument(name, nameof(name)).NotEmpty();
            CreatedBy = Guard.Argument(userId, nameof(userId)).NotEmpty();
            ConcurrencyToken = concurrencyToken;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsCancelled { get; set; }
        public bool IsProvisioned { get; set; }
        public Guid ApiKey { get; set; }
        public int CategoryId { get; set; }
        public int ProductId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }

        [Timestamp]
        public byte[] ConcurrencyToken { get; set; }
    }
}
