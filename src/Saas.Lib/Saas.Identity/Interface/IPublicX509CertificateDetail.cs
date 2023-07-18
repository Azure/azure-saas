
namespace Saas.Identity.Interface;
public interface IPublicX509CertificateDetail
{
    string Kid { get; }
    Uri Id { get; }
    string Name { get; }
}
