using Azure.Core;
using Azure.Security.KeyVault.Keys.Cryptography;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Saas.Interface;
using Saas.Identity.Interface;
using Saas.Identity.Model;
using Saas.Identity.Crypto.Util;

namespace Saas.Identity.Crypto;
public class ClientAssertionSigningProvider(
    IMemoryCache menoryCache,
    ILogger<ClientAssertionSigningProvider> logger,
    IPublicX509CertificateDetailProvider publicX509CertificateDetailProvider) : IClientAssertionSigningProvider
{
    private readonly ILogger _logger = logger;

    // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/loggermessage?view=aspnetcore-7.0
    private static readonly Action<ILogger, Exception> _logError = LoggerMessage.Define(
            LogLevel.Error,
            new EventId(1, nameof(ClientAssertionSigningProvider)),
            "Client Assertion Signing Provider");

    private readonly IMemoryCache _memoryCache = menoryCache;
    private readonly IPublicX509CertificateDetailProvider _publicX509CertificateDetailProvider = publicX509CertificateDetailProvider;

    public async Task<string> GetClientAssertion(string keyVaultUrl,
        string certKeyName,
        string audience,
        string clientId,
        TokenCredential credential,
        TimeSpan lifetime = default) =>
            await GetClientAssertion(
                new KeyInfo(keyVaultUrl, certKeyName),
                audience,
                clientId,
                credential,
                lifetime);

    public async Task<string> GetClientAssertion(
        IKeyVaultInfo keyInfo,
        string audience,
        string clientId,
        TokenCredential credential,
        TimeSpan lifetime = default)
    {
        var cacheItemName = $"{keyInfo.KeyVaultUrl}-{keyInfo.KeyVaultCertificateName}-{clientId}-{audience}";

        if (_memoryCache.TryGetValue<string>(cacheItemName, out var clientAssertion)
            && clientAssertion is not null)
        {
            _logger.LogInformation("Cache item found: {cacheItemName}", cacheItemName);
            return clientAssertion;
        }

        (clientAssertion, DateTimeOffset expiryTime) =
            await GetClientAssertionFromKeyVault(keyInfo, audience, clientId, credential, lifetime);

        // cached assertion expires with there's approx 10 % left of it's life time. 
        var cacheExpiration = expiryTime - (expiryTime - DateTimeOffset.UtcNow) / 10;

        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(cacheExpiration);

        _memoryCache.Set(cacheItemName, clientAssertion, cacheOptions);

        return clientAssertion;
    }

    private async Task<(string clientAssertion, DateTimeOffset expiryTime)> GetClientAssertionFromKeyVault(
        IKeyVaultInfo keyInfo,
        string audience,
        string clientId,
        TokenCredential credential,
        TimeSpan lifetime = default)
    {
        var validFrom = DateTimeOffset.UtcNow;
        var expiryTime = DateTimeOffset.UtcNow + lifetime;

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

        try
        {
            var signResult = await keyVaultCryptoClient.SignAsync(SignatureAlgorithm.RS256, digest);

            return (
                $"{unsignedAssertion}.{signResult.Signature.Base64UrlEncode()}",
                expiryTime);
        }
        catch (Exception ex)
        {
            _logError(_logger, ex);
            throw;
        }
    }

    private async Task<(string unsignedAssertion, IPublicX509CertificateDetail publicCertDetails)> CreateUnsignedAssertion(
            IKeyVaultInfo keyInfo,
            Claim[] claims,
            TokenCredential credential)
    {
        try
        {
            var publicCertDetails =
        await _publicX509CertificateDetailProvider.GetX509Detail(keyInfo, credential);

            var headerJson = $$"""{"alg":"RS256","typ":"JWT","x5t":"{{publicCertDetails.Kid}}"}""";

            JwtPayload payloadJwt = new(claims);

            var header = Base64UrlEncoder.Encode(headerJson);
            var payload = Base64UrlEncoder.Encode(JsonSerializer.Serialize(payloadJwt));

            return ($"{header}.{payload}", publicCertDetails);
        }
        catch (Exception ex)
        {
            _logError(_logger, ex);
            throw;
        }
    }
}
