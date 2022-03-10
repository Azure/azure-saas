using Dawn;

using Saas.Admin.Service.Data;

namespace Saas.Admin.Service.Controllers
{
    public class SubscriptionDTO
    {
        /// <summary>
        /// For serialization don't use this
        /// </summary>
        public SubscriptionDTO()
        {
            Name = String.Empty;
            CreatedBy = String.Empty;
            Version = String.Empty;
        }

        public SubscriptionDTO(Subscription subscription)
        {
            Id = subscription.Id;
            IsActive = subscription.IsActive;
            IsCancelled = subscription.IsCancelled;
            IsProvisioned = subscription.IsProvisioned;
            ApiKey = subscription.ApiKey;
            CategoryId = subscription.CategoryId;
            ProductId = subscription.ProductId;
            CreatedTime = subscription.CreatedTime;

            Name = Guard.Argument(subscription.Name, nameof(subscription.Name)).NotEmpty();
            CreatedBy = Guard.Argument(subscription.CreatedBy, nameof(subscription.CreatedBy)).NotEmpty();
            Version = Convert.ToBase64String(Guard.Argument(subscription.ConcurrencyToken, nameof(subscription.ConcurrencyToken)).NotEmpty());
        }

        public Subscription ToSubscription()
        {

            byte[] concurrentcyToken = Convert.FromBase64String(this.Version);
            Subscription subscription = new Subscription(this.Name, this.CreatedBy, concurrentcyToken)
            {
                Id = this.Id,
                IsActive = this.IsActive,
                IsCancelled = this.IsCancelled,
                IsProvisioned = this.IsProvisioned,
                ApiKey = this.ApiKey,
                CategoryId = this.CategoryId,
                ProductId = this.ProductId,
                CreatedTime = this.CreatedTime,
            };
            return subscription;
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
        public string Version { get; set; }
    }

    public class SubscriptionDTOPage
    {
        public SubscriptionDTOPage(IEnumerable<SubscriptionDTO> subscriptions, int totalCount, int startIndex)
        {
            Subscriptions = subscriptions;
            TotalCount = totalCount;
            StartIndex = startIndex;
        }

        public IEnumerable<SubscriptionDTO> Subscriptions { get; }
        public int TotalCount { get; }
        public int StartIndex { get; }
    }
}
