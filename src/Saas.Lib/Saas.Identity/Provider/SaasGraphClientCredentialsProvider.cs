﻿
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Saas.Shared.Interface;
using Saas.Shared.Options;
using System.Net.Http.Headers;

namespace Saas.Identity.Provider;
public class SaasGraphClientCredentialsProvider<TOptions> : IAuthenticationProvider
    where TOptions : AzureAdB2CBase
{
    private readonly ILogger _logger;
    private readonly SaasApiAuthenticationProvider<ISaasMicrosoftGraphApi, TOptions> _authProvider;

    // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/logging/loggermessage?view=aspnetcore-7.0
    private static readonly Action<ILogger, Exception> _logError = LoggerMessage.Define(
            LogLevel.Error,
            new EventId(1, nameof(SaasGraphClientCredentialsProvider<TOptions>)),
            "Client Assertion Signing Provider");

    public SaasGraphClientCredentialsProvider(
        SaasApiAuthenticationProvider<ISaasMicrosoftGraphApi, TOptions> authProvider,
        ILogger<SaasGraphClientCredentialsProvider<TOptions>> logger)
    {
        _logger = logger;
        _authProvider = authProvider;
    }

    public async Task AuthenticateRequestAsync(HttpRequestMessage requestMessage)
    {
        try
        {
            requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue("bearer", await _authProvider.GetAccessTokenAsync());
        }
        catch (Exception ex)
        {
            _logError(_logger, ex);
            throw;
        }
    }
}
