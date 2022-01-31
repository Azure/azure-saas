using System;
using System.Collections.Generic;

namespace Saas.Catalog.Api.Models
{
    public partial class Tenant
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }
        public bool IsCancelled { get; set; }
        public bool IsProvisioned { get; set; }
        public Guid ApiKey { get; set; }
        public int CategoryId { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; }
        public DateTime Created { get; set; }
    }
}
