using System.Net.Http;

namespace Saas.SignupAdministration.Web.Services;

public class Email : IEmail
{
    ILogger<Email> _logger; 
    private readonly EmailOptions _options;
    private readonly IHttpClientFactory _client; 

    public Email(ILogger<Email> logger, IOptions<EmailOptions> options, IHttpClientFactory client)
    {
        _logger = logger;
        _options = options.Value;
        _client = client; 
    }

    public async Task<HttpResponseMessage> SendAsync(string recipientAddress)
    {
        HttpResponseMessage rtn = new HttpResponseMessage();
        var client = _client.CreateClient(_options.EndPoint);
        JSONEmail email = new JSONEmail();
        email.HTML = _options.Body;
        email.Subject = _options.Subject;
        email.EmailFrom = _options.FromAddress;
        email.EmailTo = recipientAddress;
        email.EmailToName = recipientAddress; 

        var json = JsonSerializer.Serialize(email);
        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
        try
        {
             rtn = await client.PostAsync(_options.EndPoint, content);
        }
        catch (Exception ex)
        {
            //Logging any errors but not letting them prevent the processes from moving forward. 
            _logger.LogWarning(ex, "Problem emailing tenant");
        }
        return rtn;
    }
}
