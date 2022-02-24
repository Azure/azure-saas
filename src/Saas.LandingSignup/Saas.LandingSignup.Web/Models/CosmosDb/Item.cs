using Newtonsoft.Json;
using System;

namespace Saas.LandingSignup.Web.Models.CosmosDb
{
    public class Item
    {
        [JsonProperty(PropertyName = SR.CosmosIdProperty)]
        public string Id { get; set; }

        [JsonProperty(PropertyName = SR.CosmosNameProperty)]
        public string Name { get; set; }

        [JsonProperty(PropertyName = SR.CosmosTenantNameProperty)]
        public string TenantName { get; set; }

        [JsonProperty(PropertyName = SR.CosmosUserIdProperty)]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = SR.CosmosIsExistingUserProperty)]
        public string IsExistingUser { get; set; }

        [JsonProperty(PropertyName = SR.CosmosCategoryIdProperty)]
        public int CategoryId { get; set; }

        [JsonProperty(PropertyName = SR.CosmosProductIdProperty)]
        public int ProductId { get; set; }

        [JsonProperty(PropertyName = SR.CosmosIsCompleteProperty)]
        public bool IsComplete { get; set; }

        [JsonProperty(PropertyName = SR.CosmosIpAddressProperty)]
        public string IpAddress { get; set; }

        [JsonProperty(PropertyName = SR.CosmosCreatedProperty)]
        public DateTime Created { get; set; }
    }
}
