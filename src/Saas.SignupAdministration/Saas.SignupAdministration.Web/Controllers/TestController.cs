using Dawn;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Web;
using Saas.Admin.Client;
using Saas.Shared.Options;
using Saas.SignupAdministration.Web.Services.StateMachine;
using static Saas.SignupAdministration.Web.Controllers.TestController;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Saas.SignupAdministration.Web.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TestController : ControllerBase
{
    private readonly IAdminServiceClient _adminServiceClient;
    private readonly IPersistenceProvider _persistenceProvider;
    private readonly IApplicationUser _applicationUser;
    private readonly IConfiguration _config;
    private readonly ITokenAcquisition _tokenAcquisition;
    private readonly IEnumerable<string> _scopes;


    public TestController(IApplicationUser applicationUser, IAdminServiceClient adminServiceClient, IPersistenceProvider persistenceProvider, IConfiguration config, ITokenAcquisition tokenAcquisition, IOptions<SaasAppScopeOptions> scopes)
    {
        _applicationUser = applicationUser;
        _adminServiceClient = adminServiceClient;
        _persistenceProvider = persistenceProvider;
        _config = config;
        _tokenAcquisition = tokenAcquisition;
        _scopes = scopes.Value.Scopes ?? throw new ArgumentNullException($"Scopes must be defined.");
    }
    // GET: api/<TestController>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        TenantDTO? tenant = _persistenceProvider.Retrieve<TenantDTO>(_applicationUser.EmailAddress);

        if (tenant == null)
        {
            //Retreieve from database
            if (!Guid.TryParse(User?.GetNameIdentifierId(), out var userId))
            {
                throw new InvalidOperationException("The the User Name Identifier must be a Guid.");
            }
            var tenants = await _adminServiceClient.TenantsAsync(userId);
            Console.WriteLine("Count of tenants is " + userId);
            if (tenants.Count < 1)  //Zero tenants
            {
                return BadRequest();
            }
            tenant = tenants.ElementAt(0);
            _persistenceProvider.Persist(_applicationUser.EmailAddress, tenant);
        }

        if (tenant.IsDbReady)
        {
            //Add product
            GetProduct(tenant.DatabaseName);

            return new JsonResult(GetProduct(tenant.DatabaseName));
        }

        return BadRequest();
    }

    private List<Product> GetProduct(string databaseName)
    {
        List<Product> products = new List<Product>();
        var sqlConnectionString = _config.GetRequiredSection(SqlOptions.SectionName)
      .Get<SqlOptions>()?.IbizzSaasConnectionString
          ?? throw new NullReferenceException("SQL Connection string cannot be null.");

        string mask = "iBusinessSaasTests";
        sqlConnectionString = sqlConnectionString.Replace(mask, databaseName);

        using (SqlConnection con = new SqlConnection(sqlConnectionString))
        {
            con.Open();
            string query = "Select * from products";
            using (SqlCommand command = new SqlCommand(query, con))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Product product = new Product
                        {
                            Id = reader.GetInt32(0),
                            Title = reader.GetString(1)
                        };

                        products.Add(product);  
                    }
                }
            }
        }

        return products;
    }

    // GET api/<TestController>/5
    [HttpGet("{id}")]
    public string Get(int id)
    {
        return "value";
    }

    // POST api/<TestController>
    [HttpPost]
    public async Task Post([FromBody] Product product)
    {

        TenantDTO? tenant = _persistenceProvider.Retrieve<TenantDTO>(_applicationUser.EmailAddress);

        if(tenant == null)
        {
            //Retreieve from database
            if (!Guid.TryParse(User?.GetNameIdentifierId(), out var userId))
            {
                throw new InvalidOperationException("The the User Name Identifier must be a Guid.");
            }
            var tenants = await _adminServiceClient.TenantsAsync(userId);

            if(tenants.Count < 1)  //Zero tenants
            {
                return;
            }
            tenant = tenants.ElementAt(0);
            _persistenceProvider.Persist(_applicationUser.EmailAddress, tenant);
        }

        if (tenant.IsDbReady)
        {
            //Add product
            addProduct(product, tenant.DatabaseName);
        }

    }

    private void addProduct(Product product, string databaseName)
    {
        var sqlConnectionString = _config.GetRequiredSection(SqlOptions.SectionName)
       .Get<SqlOptions>()?.IbizzSaasConnectionString
           ?? throw new NullReferenceException("SQL Connection string cannot be null.");

        string mask = "iBusinessSaasTests";
        sqlConnectionString = sqlConnectionString.Replace(mask, databaseName);

        using(SqlConnection con = new SqlConnection(sqlConnectionString))
        {
            con.Open();
            string query = $"Insert Into products values('{product.Title}')";
            using(SqlCommand command = new SqlCommand(query, con))
            {
                command.ExecuteReader();
            }
        }


    }

    [HttpGet]
    [Route("/gettoken")]
    public async Task<JsonResult> GetToken()
    {
        string accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(_scopes);
        string? id = User.GetNameIdentifierId();
        return new JsonResult(new {accessToken, id});
    }

    // DELETE api/<TestController>/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
    }

    public class Product
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
    }
}
