using System.Text.Json.Serialization;
namespace Saas.Permissions.Api.Models;

public class ADB2CRequest
{
    [JsonPropertyName("signInNames.emailAddress")]
    public string EmailAddress { get; set; } = string.Empty;
    public Guid ObjectId { get; set; }
}
