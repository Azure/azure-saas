using System.Text.Json.Serialization;

namespace Saas.SignupAdministration.Web.Models
{
    [Serializable]
    public class JSONEmail
    {
        [JsonPropertyName("HTML")]
        public string HTML { get; set; } = string.Empty;
        [JsonPropertyName("emailFrom")]
        public string EmailFrom { get; set; } = string.Empty;
        [JsonPropertyName("emailTo")]
        public string EmailTo { get; set; } = string.Empty;
        [JsonPropertyName("emailToName")]
        public string EmailToName { get; set; } = string.Empty;
        [JsonPropertyName("subject")]
        public string Subject { get; set; } = string.Empty;
    }
}