namespace Saas.Admin.Service.Data.AppSettings
{
    public class PermissionsApiOptions
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string[] Scopes { get; set; } = new string[0];
    }
}
