using Azure.Core;
using Azure.Security.KeyVault.Certificates;
using ClientAssertionWithKeyVault.Interface;
using ClientAssertionWithKeyVault.Model;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography.X509Certificates;
using ClientAssertionWithKeyVault.Util;

namespace ClientAssertionWithKeyVault;
public class PublicX509CertificateDetailProvider : IPublicX509CertificateDetailProvider
{
    private readonly IMemoryCache _memoryCache;
    
    public PublicX509CertificateDetailProvider(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public async Task<IPublicX509CertificateDetail> GetX509Detail(IKeyInfo keyInfo, TokenCredential credential)
    {
        if (! Uri.TryCreate(keyInfo.KeyVaultUrl, UriKind.Absolute, out Uri? keyVaultUri))
        {
            throw new UriFormatException($"Invalid Key Vault URL: '{keyInfo.KeyVaultUrl}'");
        }

        var cacheItemName = $"{keyInfo.KeyVaultUrl}-{keyInfo.KeyVaultCertificateName}";

        if (_memoryCache.TryGetValue<PublicX509CertificateDetail>(cacheItemName, out var cachedCertDetails))
        {
            return cachedCertDetails
                ?? throw new NullReferenceException("Memory caching error. Certificate in cache cannot be null.");
        }

        CertificateClient certClient = new(keyVaultUri, credential);

        Azure.Response<KeyVaultCertificateWithPolicy> keyVaultPublicCertResponse = 
            await certClient.GetCertificateAsync(keyInfo.KeyVaultCertificateName);

        X509Certificate2 publicX509Cert = new(keyVaultPublicCertResponse.Value.Cer);

        PublicX509CertificateDetail publicX509CertificateDetail = new(
            publicX509Cert.GetCertHash().Base64UrlEncode(),
            keyVaultPublicCertResponse.Value.KeyId,
            keyVaultPublicCertResponse.Value.Name);

        // The certificate details are cached for 24 hours since they rarely change. 
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromHours(24));

        _memoryCache.Set(cacheItemName, publicX509CertificateDetail, cacheOptions);

        return publicX509CertificateDetail;
    }
}
