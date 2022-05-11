using System.Text.Json.Serialization;
namespace Saas.Permissions.Service.Models;

public class ClaimsRequest
{
    [JsonPropertyName("signInNames.emailAddress")]
    public string EmailAddress { get; set; } = string.Empty;
    public Guid ObjectId { get; set; }
    
    public string ClientId { get; set; } = string.Empty;
}
