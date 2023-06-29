using Microsoft.Data.SqlClient;
using Saas.Admin.Client;
using Saas.Shared.Options;
using Saas.SignupAdministration.Web.Services;
using System.Collections.Generic;
using System.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Saas.SignupAdministration.Web.Controllers;
[ApiController]
[Route("api/[controller]")]
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

    [HttpGet]
    [Route("claims")]
    public JsonResult ClaimsCheck()
    {
        int count = User.Claims.Count();

        IDictionary<string, string> claims = new Dictionary<string, string>();

        int se = 0;

        foreach (var item in User.Claims)
        {
            claims.Add(item.Type + se++, item.Value);
        }

        return new JsonResult(new { count, claims });
    }
    // GET: api/<TestController>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        GetBooking("tenant_Spike");

        return new JsonResult(GetBooking("tenant_Spike"));

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

            if (tenant.IsDbReady)//Persist only when db is ready
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

    private List<Dictionary<string, string>> GetBooking(string databaseName)
    {
        List<Dictionary<string, string>> bookings = new List<Dictionary<string, string>>();


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
                        bookings.Add(customBooking(new BookingDto
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

                        }));
                    }
                }
            }
        }

        return bookings;
    }

    private SingleBookingDto GetBookingById(string databaseName, int id)
    {
        SingleBookingDto booking = null;
        var sqlConnectionString = _config.GetRequiredSection(SqlOptions.SectionName)
      .Get<SqlOptions>()?.IbizzSaasConnectionString
          ?? throw new NullReferenceException("SQL Connection string cannot be null.");

        string mask = "iBusinessSaasTests";
        sqlConnectionString = sqlConnectionString.Replace(mask, databaseName);

        using (SqlConnection con = new SqlConnection(sqlConnectionString))
        {
            con.Open();

            using (SqlCommand command = new SqlCommand("getBookingByID", con))
            {

                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("bookingId", SqlDbType.Int).Value = id;

                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        booking = (new SingleBookingDto
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
                            UserId = reader.GetInt64(9),
                            DisabilityStatus = reader.GetString(18),
                            Email = reader.GetString(11),
                            EmployerName = reader.GetString(15),
                            Experience = reader.GetInt32(16),
                            FullName = reader.GetString(10),
                            IdNumber = reader.GetString(19),
                            PhysicalAddress = reader.GetString(12),
                            Position = reader.GetString(17),
                            Telephone = reader.GetString(13),
                            OriginCountry = reader.GetString(14)

                        });
                    }

                    reader.Close();
                }
            }
        }

        return booking;
    }


    // GET api/<TestController>/5
    [HttpGet("{id}")]
    public async Task<ActionResult<SingleBookingDto>> Get(int id)
    {
        //Add booking
        GetBooking("tenant_Spike");

        return new JsonResult(GetBookingById("tenant_Spike", id));
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

            if (tenant.IsDbReady)//Persist only when db is ready
                _persistenceProvider.Persist(_applicationUser.EmailAddress, tenant);
        }

        if (tenant.IsDbReady)
        {
            //Add booking
            GetBooking(tenant.DatabaseName);

            return new JsonResult(GetBookingById("tenant_Spike", id));
        }


        return BadRequest();
    }

    // POST api/<TestController>
    [HttpPost]
    public async Task<ActionResult> Post([FromBody] Booking booking)
    {

        List<Booking> bookings = new();
        //Add booking
        CreateBooking(booking, "tenant_Spike");
        var results = GetBooking("tenant_Spike");

        return new JsonResult(results.Last());

        TenantDTO? tenant = _persistenceProvider.Retrieve<TenantDTO>(_applicationUser.EmailAddress);

        if (tenant == null)
        {
            //Retreieve from database
            if (!Guid.TryParse(User?.GetNameIdentifierId(), out var userId))
            {
                throw new InvalidOperationException("The the User Name Identifier must be a Guid.");
            }
            var tenants = await _adminServiceClient.TenantsAsync(userId);

            if (tenants.Count < 1)  //Zero tenants
            {
                return new JsonResult(bookings);
            }
            tenant = tenants.ElementAt(0);

            if (tenant.IsDbReady)
                _persistenceProvider.Persist(_applicationUser.EmailAddress, tenant);
        }

        if (tenant.IsDbReady)
        {
            //Add booking
            CreateBooking(booking, tenant.DatabaseName);
            //var results = GetOrder(tenant.DatabaseName);

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
    [HttpPut]

    public async Task<ActionResult> Put(SingleBookingDto booking)
    {
        //Update booking

        var results = UpdateBooking(booking, "tenant_Spike");

        return new JsonResult(results);
    }

    private void CreateBooking(Booking booking, string databaseName)
    {
        var sqlConnectionString = _config.GetRequiredSection(SqlOptions.SectionName)
       .Get<SqlOptions>()?.IbizzSaasConnectionString
           ?? throw new NullReferenceException("SQL Connection string cannot be null.");

        string mask = "iBusinessSaasTests";
        sqlConnectionString = sqlConnectionString.Replace(mask, databaseName);

        using (SqlConnection con = new SqlConnection(sqlConnectionString))
        {
            con.Open();

            using (SqlCommand command = new SqlCommand("spInsertBookings", con))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("bookingId", SqlDbType.BigInt).Value = 0;
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

    private SingleBookingDto UpdateBooking(SingleBookingDto booking, string databaseName)
    {
        var sqlConnectionString = _config.GetRequiredSection(SqlOptions.SectionName)
       .Get<SqlOptions>()?.IbizzSaasConnectionString
           ?? throw new NullReferenceException("SQL Connection string cannot be null.");

        string mask = "iBusinessSaasTests";
        sqlConnectionString = sqlConnectionString.Replace(mask, databaseName);

        using (SqlConnection con = new SqlConnection(sqlConnectionString))
        {
            con.Open();

            using (SqlCommand command = new SqlCommand("spUpdateBooking", con))
            {
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("bookingId", SqlDbType.BigInt).Value = booking.BookingId;
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
        return booking;

    }

    [HttpGet]
    [Route("/gettoken")]
    public async Task<JsonResult> GetToken()
    {
        string accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(_scopes);
        string? id = User.GetNameIdentifierId();
        return new JsonResult(new { accessToken, id });
    }

    [HttpPost]
    [Route("/posttest")]
    [ValidateAntiForgeryToken]
    public string PostTest([FromForm] string abc)
    {

        return "Your response " + abc;
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

    //Used to create a custom booking to suit current design
    //That is returning specific data as specificed in the front-end
    private Dictionary<string, string> customBooking(BookingDto bookingDto)
    {
        Dictionary<string, string> customBooking = new Dictionary<string, string>
            {
                { "bookingId", "" + bookingDto.BookingId },
                { "externalSchemeAdmin", bookingDto.ExternalSchemeAdmin },
                { "bookingType", bookingDto.BookingType },
                { "retirementSchemeName", bookingDto.RetirementSchemeName },
                { "schemePosition", bookingDto.SchemePosition },
                { "trainingVenue", bookingDto.TrainingVenue },
                { "paymentMode", bookingDto.PaymentMode },
                { "additionalRequirements", bookingDto.AdditionalRequirements },
                { "userId", "" + bookingDto.UserId }
            };

        return customBooking;
    }
}

