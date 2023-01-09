using Azure.Core;
using Azure.Security.KeyVault.Keys.Cryptography;
using ClientAssertionWithKeyVault.Interface;
using ClientAssertionWithKeyVault.Model;
using ClientAssertionWithKeyVault.Util;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ClientAssertionWithKeyVault;
public class ClientAssertionSigningProvider : IClientAssertionSigningProvider
{
    private readonly IMemoryCache _memoryCache;
    private readonly IPublicX509CertificateDetailProvider _publicX509CertificateDetailProvider;

    public ClientAssertionSigningProvider(
        IMemoryCache menoryCache,
        IPublicX509CertificateDetailProvider? publicX509CertificateDetailProvider = null)
    {
        _memoryCache = menoryCache;

        _publicX509CertificateDetailProvider = publicX509CertificateDetailProvider is null 
            ? new PublicX509CertificateDetailProvider(menoryCache)
            : publicX509CertificateDetailProvider;
    }
   
    public async Task<string> GetClientAssertion(string keyVaultUrl,
        string certKeyName,
        string audience,
        string clientId,
        TokenCredential credential,
        TimeSpan lifetime = default)
    {
        IKeyInfo keyInfo = new KeyInfo(keyVaultUrl, certKeyName);

        return await GetClientAssertion(keyInfo, audience, clientId, credential, lifetime);
    }

    public async Task<string> GetClientAssertion(
        IKeyInfo keyInfo,
        string audience,
        string clientId,
        TokenCredential credential,
        TimeSpan lifetime = default)
    {
        var cacheItemName = $"{keyInfo.KeyVaultUrl}-{keyInfo.KeyVaultCertificateName}-{clientId}-{audience}";

        if (_memoryCache.TryGetValue<string>(cacheItemName, out var clientAssertion))
        {
            return clientAssertion
                ?? throw new NullReferenceException("Memory caching error. Assertion in cache cannot be null.");
        }

        (clientAssertion, DateTimeOffset expiryTime) = 
            await GetClientAssertionFromKeyVault(keyInfo, audience, clientId, credential, lifetime);

        // cached assertion expires with there's approx 10 % left of it's life time. 
        var cacheExpiration = expiryTime - ((expiryTime - DateTimeOffset.UtcNow) / 10);

        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(cacheExpiration);

        _memoryCache.Set<string>(cacheItemName, clientAssertion, cacheOptions);

        return clientAssertion;
    }

    private async Task<(string clientAssertion, DateTimeOffset expiryTime)> GetClientAssertionFromKeyVault(
        IKeyInfo keyInfo,
        string audience,
        string clientId,
        TokenCredential credential,
        TimeSpan lifetime = default)
    {
        var validFrom = DateTimeOffset.UtcNow;
        var expiryTime = (DateTimeOffset.UtcNow + lifetime);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Iss, clientId),
            new Claim(JwtRegisteredClaimNames.Sub, clientId),
            new Claim(JwtRegisteredClaimNames.Aud, audience),
            new Claim(JwtRegisteredClaimNames.Exp, expiryTime.ToUnixTimeSeconds().ToString()),
            new Claim(JwtRegisteredClaimNames.Nbf, validFrom.ToUnixTimeSeconds().ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        (string unsignedAssertion, IPublicX509CertificateDetail publicCertDetails)
            = await CreateUnsignedAssertion(keyInfo, claims, credential);

        CryptographyClient keyVaultCryptoClient = new(publicCertDetails.Id, credential);

        var digest = SHA256.HashData(Encoding.UTF8.GetBytes(unsignedAssertion));

        var signResult = await keyVaultCryptoClient.SignAsync(SignatureAlgorithm.RS256, digest);

        return (
            $"{unsignedAssertion}.{signResult.Signature.Base64UrlEncode()}",
            expiryTime);
    }

    private async Task<(string unsignedAssertion, IPublicX509CertificateDetail publicCertDetails)> CreateUnsignedAssertion(
            IKeyInfo keyInfo,
            Claim[] claims,
            TokenCredential credential)
    {
        var publicCertDetails = 
            await _publicX509CertificateDetailProvider.GetX509Detail(keyInfo, credential);

        var headerJson = $$"""{"alg":"RS256","typ":"JWT","x5t":"{{publicCertDetails.Kid}}"}""";

        JwtPayload payloadJwt = new(claims);

        var header = Base64UrlEncoder.Encode(headerJson);
        var payload = Base64UrlEncoder.Encode(JsonSerializer.Serialize(payloadJwt));

        return ($"{header}.{payload}", publicCertDetails);
    }
}
