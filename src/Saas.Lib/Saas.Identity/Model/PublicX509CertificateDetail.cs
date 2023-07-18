using Saas.Identity.Interface;

namespace Saas.Identity.Model;
public record PublicX509CertificateDetail : IPublicX509CertificateDetail
{
    public PublicX509CertificateDetail(string kid, Uri id, string name)
    {
        Kid = kid;
        Id = id;
        Name = name;
    }

    public string Kid { get; init; }
    public Uri Id { get; init; }
    public string Name { get; init; }
}
