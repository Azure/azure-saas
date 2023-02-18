using Azure.Core;
using Saas.Interface;

namespace Saas.Identity.Interface;

public interface IPublicX509CertificateDetailProvider
{
    Task<IPublicX509CertificateDetail> GetX509Detail(IKeyVaultInfo keyInfo, TokenCredential credential);
}
