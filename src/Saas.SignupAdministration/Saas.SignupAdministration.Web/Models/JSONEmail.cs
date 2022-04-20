namespace Saas.SignupAdministration.Web.Models
{
    [Serializable]
    public class JSONEmail
    {
        public string HTML { get; set; }
        public string emailFrom { get; set; }
        public string emailTo { get; set; }
        public string emailToName { get; set; }
        public string subject { get; set; }
    }
}
