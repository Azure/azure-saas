using Azure.Core;
using Azure.Security.KeyVault.Certificates;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using Saas.Interface;
using Saas.Identity.Interface;
using Saas.Identity.Model;
using Saas.Identity.Crypto.Util;

namespace Saas.Identity.Crypto;
public class PublicX509CertificateDetailProvider(
    IMemoryCache memoryCache,
    ILogger<PublicX509CertificateDetailProvider> logger) : IPublicX509CertificateDetailProvider
{
    private readonly ILogger _logger = logger;

    // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/loggermessage?view=aspnetcore-8.0
    private static readonly Action<ILogger, Exception> _logError = LoggerMessage.Define(
            LogLevel.Error,
            new EventId(1, nameof(PublicX509CertificateDetailProvider)),
            "Client Assertion Signing Provider");

    private readonly IMemoryCache _memoryCache = memoryCache;

    public async Task<IPublicX509CertificateDetail> GetX509Detail(IKeyVaultInfo keyInfo, TokenCredential credential)
    {
        if (!Uri.TryCreate(keyInfo.KeyVaultUrl, UriKind.Absolute, out Uri? keyVaultUri))
        {
            throw new UriFormatException($"Invalid Key Vault Url format: '{keyInfo.KeyVaultUrl}'");
        }

        var cacheItemName = $"{keyInfo.KeyVaultUrl}-{keyInfo.KeyVaultCertificateName}";

        if (_memoryCache.TryGetValue<PublicX509CertificateDetail>(cacheItemName, out var cachedCertDetails)
            && cachedCertDetails is not null)
        {
            return cachedCertDetails;
        }

        try
        {
            CertificateClient certClient = new(keyVaultUri, credential);

            Azure.Response<KeyVaultCertificateWithPolicy> keyVaultPublicCertResponse =
                await certClient.GetCertificateAsync(keyInfo.KeyVaultCertificateName);

            X509Certificate2 publicX509Cert = new(keyVaultPublicCertResponse.Value.Cer);

            PublicX509CertificateDetail publicX509CertificateDetail = new(
                publicX509Cert.GetCertHash().Base64UrlEncode(),
                keyVaultPublicCertResponse.Value.KeyId,
                keyVaultPublicCertResponse.Value.Name);

            // The certificate details are cached for 2 hours since they rarely change. 
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(2));

            _memoryCache.Set(cacheItemName, publicX509CertificateDetail, cacheOptions);

            return publicX509CertificateDetail;
        }
        catch (Exception ex)
        {
            _logError(_logger, ex);
            throw;
        }
    }
}
