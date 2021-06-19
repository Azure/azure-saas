using Newtonsoft.Json;
using System;

namespace Saas.Provider.Web.Models.CosmosDb
{
    public class Item
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "tenantName")]
        public string TenantName { get; set; }

        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "isExistingUser")]
        public string IsExistingUser { get; set; }

        [JsonProperty(PropertyName = "categoryId")]
        public int CategoryId { get; set; }

        [JsonProperty(PropertyName = "productId")]
        public int ProductId { get; set; }

        [JsonProperty(PropertyName = "isComplete")]
        public bool IsComplete { get; set; }

        [JsonProperty(PropertyName = "ipAddress")]
        public string IpAddress { get; set; }

        [JsonProperty(PropertyName = "created")]
        public DateTime Created { get; set; }
    }
}
