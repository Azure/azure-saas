﻿
namespace Saas.SignupAdministration.Web.Models
{
    public class AppSettings
    {
        public string RedirectUri { get; set; } = string.Empty;
        public string AdminServiceBaseUrl { get; set; } = string.Empty;
        public string AdminServiceScopes { get; set; } = string.Empty;
    }
}
