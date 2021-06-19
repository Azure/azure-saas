namespace Saas.Provider.Web.Models
{
    public class AppSettings
    {
        public string RedirectUri { get; set; }
        public string SendGridAPIKey { get; set; }
        public string StripePublishableKey { get; set; }
        public string StripeSecretKey { get; set; }
        public string StripeProductPlanSubscriberBasic { get; set; }
        public string StripeProductPlanSubscriberStandard { get; set; }
    }
}
