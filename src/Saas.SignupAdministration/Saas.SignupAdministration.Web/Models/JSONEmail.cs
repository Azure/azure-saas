using System.Text.Json.Serialization;

namespace Saas.SignupAdministration.Web.Models
{
    [Serializable]
    public class JSONEmail
    {
        [JsonPropertyName("HTML")]
        public string HTML { get; set; }
        [JsonPropertyName("emailFrom")]
        public string EmailFrom { get; set; }
        [JsonPropertyName("emailTo")]
        public string EmailTo { get; set; }
        [JsonPropertyName("emailToName")]
        public string EmailToName { get; set; }
        [JsonPropertyName("subject")]
        public string Subject { get; set; }
    }
}