using Azure.Core;
using Saas.Interface;

namespace ClientAssertionWithKeyVault.Interface;

public interface IPublicX509CertificateDetailProvider
{
    Task<IPublicX509CertificateDetail> GetX509Detail(IKeyInfo keyInfo, TokenCredential credential);
}
