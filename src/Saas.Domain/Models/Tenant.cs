using Dawn;

using System.Runtime.Versioning;

namespace Saas.Domain.Models
{
    public class Tenant
    {
        public Tenant(string name, string userId)
        {
            this.Name = Guard.Argument(name, nameof(name)).NotEmpty();
            this.UserId = Guard.Argument(userId, nameof(userId)).NotEmpty();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsCancelled { get; set; }
        public bool IsProvisioned { get; set; }
        public Guid ApiKey { get; set; }
        public int CategoryId { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; }
        public DateTime Created { get; set; }
    }
}
