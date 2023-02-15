using Azure.Core;
using Saas.Interface;

namespace Saas.Identity.Interface;

public interface IPublicX509CertificateDetailProvider
{
    Task<IPublicX509CertificateDetail> GetX509Detail(IKeyInfo keyInfo, TokenCredential credential);
}
