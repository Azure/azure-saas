using Azure.Core;

namespace ClientAssertionWithKeyVault.Interface;

public interface IPublicX509CertificateDetailProvider
{
    Task<IPublicX509CertificateDetail> GetX509Detail(IKeyInfo keyInfo, TokenCredential credential);
}
