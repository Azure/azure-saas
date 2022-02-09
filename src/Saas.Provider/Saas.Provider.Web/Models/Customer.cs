using System;

namespace Saas.Provider.Web.Models
{
    public class Customer
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string CustomerName { get; set; }
        public bool IsActive { get; set; }
    }
}
