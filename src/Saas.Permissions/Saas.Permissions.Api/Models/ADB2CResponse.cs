using System.Text.Json.Serialization;

namespace Saas.Permissions.Api.Models
{


    public class ResponseContent
    {
        public const string ApiVersion = "1.0.0";

        public ResponseContent()
        {
            this.version = ResponseContent.ApiVersion;
            this.action = "Continue";
        }

        public ResponseContent(string action, string userMessage)
        {
            this.version = ResponseContent.ApiVersion;
            this.action = action;
            this.userMessage = userMessage;
            if (action == "ValidationError")
            {
                this.status = "400";
            }
        }

        public string version { get; }
        public string action { get; set; }


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string userMessage { get; set; }


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string status { get; set; }


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string jobTitle { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string> roles { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string extension_CustomClaim { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string scp { get; set; }
    }

}
