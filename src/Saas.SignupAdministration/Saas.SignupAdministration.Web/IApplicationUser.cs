namespace Saas.SignupAdministration.Web
{
    public interface IApplicationUser
    {
        public string EmailAddress { get; }
        public Guid NameIdentifier { get; }
        public string AuthenticationClassReference { get; }
        public DateTime AuthenticationTime { get; }
        public long AuthenticationTimeTicks { get; }
        public string GivenName { get; }
        public string Surname { get; }
        public Guid TenantId { get; }
    }
}
