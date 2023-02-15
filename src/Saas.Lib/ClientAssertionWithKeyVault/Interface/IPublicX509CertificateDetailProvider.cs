using Azure.Core;
using Microsoft.Identity.Web;

namespace ClientAssertionWithKeyVault.Interface;

public interface IPublicX509CertificateDetailProvider
{
    Task<IPublicX509CertificateDetail> GetX509Detail(CertificateDescription keyInfo, TokenCredential credential);
}
