using Microsoft.Data.SqlClient;
using Saas.Admin.Client;
using Saas.Shared.Options;
using System.Data;

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
    private readonly IUserBookingService _userBookingService;
    private readonly IEnumerable<string> _scopes;


    public TestController(IApplicationUser applicationUser, IAdminServiceClient adminServiceClient, IPersistenceProvider persistenceProvider, IConfiguration config, ITokenAcquisition tokenAcquisition, IOptions<SaasAppScopeOptions> scopes, IUserBookingService userBookingService)
    {
        _applicationUser = applicationUser;
        _adminServiceClient = adminServiceClient;
        _persistenceProvider = persistenceProvider;
        _config = config;
        _tokenAcquisition = tokenAcquisition;
        _userBookingService = userBookingService;
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

            if(tenant.IsDbReady)//Persist only when db is ready
                _persistenceProvider.Persist(_applicationUser.EmailAddress, tenant);
        }

        if (tenant.IsDbReady)
        {
            //Add booking
            GetBooking(tenant.DatabaseName);

            return new JsonResult(GetBooking(tenant.DatabaseName));
        }
        

        return BadRequest();
    }

    private List<BookingDto> GetBooking(string databaseName)
    {
        List<BookingDto> bookings = new ();
        var sqlConnectionString = _config.GetRequiredSection(SqlOptions.SectionName)
      .Get<SqlOptions>()?.IbizzSaasConnectionString
          ?? throw new NullReferenceException("SQL Connection string cannot be null.");

        string mask = "iBusinessSaasTests";
        sqlConnectionString = sqlConnectionString.Replace(mask, databaseName);

        using (SqlConnection con = new SqlConnection(sqlConnectionString))
        {
            con.Open();
     
            using (SqlCommand command = new SqlCommand("spSelectAllBookings", con))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        bookings.Add(new BookingDto
                        {
                            BookingId = reader.GetInt64(0),
                            ExternalSchemeAdmin = reader.GetString(1),
                            CourseDate = reader.GetString(2),
                            BookingType = reader.GetString(3),
                            RetirementSchemeName = reader.GetString(4),
                            SchemePosition = reader.GetString(5),
                            TrainingVenue = reader.GetString(6),
                            PaymentMode = reader.GetString(7),
                            AdditionalRequirements = reader.GetString(8),
                            UserId = reader.GetInt64(9)

                        });
                    }
                }
            }
        }

        return bookings;
    }

    // GET api/<TestController>/5
    [HttpGet("{id}")]
    public string Get(int id)
    {
        return "value";
    }

    // POST api/<TestController>
    [HttpPost]
    public async Task<ActionResult> Post([FromBody] Booking booking)
    {
        List<Booking> bookings = new();

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
                return new JsonResult(bookings);
            }
            tenant = tenants.ElementAt(0);

            if(tenant.IsDbReady)
                _persistenceProvider.Persist(_applicationUser.EmailAddress, tenant);
        }

        if (tenant.IsDbReady)
        {
            //Add booking
            CreateBooking(booking, tenant.DatabaseName);
           List<BookingDto> results = GetBooking(tenant.DatabaseName);

            return new JsonResult(results.Last());
        }
        else
        {
            
            try
            {
                booking.id = "" + DateTime.Now.Ticks;
                booking.PartitionKey = tenant.Id.ToString();
                Booking result = await _userBookingService.CreateBookingAsync(booking, booking.PartitionKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new BadRequestResult();
            }

        }

        return new JsonResult(bookings);

    }

    private void CreateBooking(Booking booking, string databaseName)
    {
        var sqlConnectionString = _config.GetRequiredSection(SqlOptions.SectionName)
       .Get<SqlOptions>()?.IbizzSaasConnectionString
           ?? throw new NullReferenceException("SQL Connection string cannot be null.");

        string mask = "iBusinessSaasTests";
        sqlConnectionString = sqlConnectionString.Replace(mask, databaseName);

        using(SqlConnection con = new SqlConnection(sqlConnectionString))
        {
            con.Open();
          
            using(SqlCommand command = new SqlCommand("spInsertBookings", con))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("bookingId", SqlDbType.Int).Value = 0;
                command.Parameters.AddWithValue("externalSchemeAdmin", SqlDbType.NVarChar).Value = booking.ExternalSchemeAdmin;
                command.Parameters.AddWithValue("courseDate", SqlDbType.NVarChar).Value = booking.CourseDate;
                command.Parameters.AddWithValue("bookingType", SqlDbType.NVarChar).Value = booking.BookingType;
                command.Parameters.AddWithValue("retirementSchemeName", SqlDbType.NVarChar).Value = booking.RetirementSchemeName;
                command.Parameters.AddWithValue("schemePosition", SqlDbType.NVarChar).Value = booking.SchemePosition;
                command.Parameters.AddWithValue("trainingVenue", SqlDbType.NVarChar).Value = booking.TrainingVenue;
                command.Parameters.AddWithValue("paymentMode", SqlDbType.NVarChar).Value = booking.PaymentMode;
                command.Parameters.AddWithValue("additionalRequirements", SqlDbType.NVarChar).Value = booking.AdditionalRequirements;
                command.Parameters.AddWithValue("userId ", SqlDbType.Int).Value = booking.UserId;
                command.Parameters.AddWithValue("disabilityStatus", SqlDbType.NVarChar).Value = booking.DisabilityStatus;
                command.Parameters.AddWithValue("email", SqlDbType.NVarChar).Value = booking.Email;
                command.Parameters.AddWithValue("employerName", SqlDbType.NVarChar).Value = booking.EmployerName;
                command.Parameters.AddWithValue("experience", SqlDbType.Int).Value = booking.Experience;
                command.Parameters.AddWithValue("fullName", SqlDbType.NVarChar).Value = booking.FullName;
                command.Parameters.AddWithValue("idNumber", SqlDbType.NVarChar).Value = booking.IdNumber;
                command.Parameters.AddWithValue("physicalAddress", SqlDbType.NVarChar).Value = booking.PhysicalAddress;
                command.Parameters.AddWithValue("position", SqlDbType.NVarChar).Value = booking.Position;
                command.Parameters.AddWithValue("originCountry", SqlDbType.NVarChar).Value = booking.OriginCountry;
                command.Parameters.AddWithValue("telephone", SqlDbType.NVarChar).Value = booking.Telephone;

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
