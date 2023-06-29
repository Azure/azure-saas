using Microsoft.Data.SqlClient;
using Saas.Admin.Client;
using Saas.Shared.Options;
using Saas.SignupAdministration.Web.Services;
using System.Collections.Generic;
using System.Data;

namespace Saas.SignupAdministration.Web.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class Test2Controller : ControllerBase
{

    private readonly IAdminServiceClient _adminServiceClient;
    private readonly IPersistenceProvider _persistenceProvider;
    private readonly IApplicationUser _applicationUser;
    private readonly IConfiguration _config;
    private readonly ITokenAcquisition _tokenAcquisition;
    private readonly IUserBookingService _userBookingService;
    private readonly IEnumerable<string> _scopes;


    public Test2Controller(IApplicationUser applicationUser, IAdminServiceClient adminServiceClient, IPersistenceProvider persistenceProvider, IConfiguration config, ITokenAcquisition tokenAcquisition, IOptions<SaasAppScopeOptions> scopes, IUserBookingService userBookingService)
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
        GetOrder("tenant_Spike");

        return new JsonResult(GetOrder("tenant_Spike"));


    }

    private List<Dictionary<string, string>> GetOrder(string databaseName)
    {
        List<Dictionary<string, string>> orders = new List<Dictionary<string, string>>();


        var sqlConnectionString = _config.GetRequiredSection(SqlOptions.SectionName)
      .Get<SqlOptions>()?.IbizzSaasConnectionString
          ?? throw new NullReferenceException("SQL Connection string cannot be null.");

        string mask = "iBusinessSaasTests";
        sqlConnectionString = sqlConnectionString.Replace(mask, databaseName);

        using (SqlConnection con = new SqlConnection(sqlConnectionString))
        {
            con.Open();

            using (SqlCommand command = new SqlCommand("spSelectAllOrders", con))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        orders.Add(customOrder(new Order
                        {
                            OrderNumber = reader.GetInt32(0),
                            CostCenter = reader.GetString(1),
                            Supplier = reader.GetString(2),
                            ShipTo = reader.GetString(3),
                            OrderDate = reader.GetDateTime(4),
                            OrderAmount = reader.GetDecimal(5),
                            DeliveryPeriod = reader.GetInt32(6),
                            FirstDeliveryDate = reader.GetDateTime(7),
                            VehicleDetails = reader.GetString(8),

                        }));
                    }
                }
            }
        }

        return orders;
    }

    private Order GetBookingById(string databaseName, int id)
    {
        Order order = null;
        var sqlConnectionString = _config.GetRequiredSection(SqlOptions.SectionName)
      .Get<SqlOptions>()?.IbizzSaasConnectionString
          ?? throw new NullReferenceException("SQL Connection string cannot be null.");

        string mask = "iBusinessSaasTests";
        sqlConnectionString = sqlConnectionString.Replace(mask, databaseName);

        using (SqlConnection con = new SqlConnection(sqlConnectionString))
        {
            con.Open();

            using (SqlCommand command = new SqlCommand("getOrderByID", con))
            {

                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("orderNumber", SqlDbType.Int).Value = id;

                using (SqlDataReader reader = command.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        order = (new Order
                        {
                            OrderNumber = reader.GetInt32(0),
                            CostCenter = reader.GetString(1),
                            Supplier = reader.GetString(2),
                            ShipTo = reader.GetString(3),
                            OrderDate = reader.GetDateTime(4),
                            OrderAmount = reader.GetDecimal(5),
                            DeliveryPeriod = reader.GetInt32(6),
                            FirstDeliveryDate = reader.GetDateTime(7),
                            VehicleDetails = reader.GetString(8),


                        });
                    }

                    reader.Close();
                }
            }
        }

        return order;
    }


    // GET api/<TestController>/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Booking>> Get(int id)
    {
        //Add booking
        GetOrder("tenant_Spike");

        return new JsonResult(GetBookingById("tenant_Spike", id));

    }

    // POST api/<TestController>
    [HttpPost]
    public async Task<ActionResult> Post([FromBody] Order order)
    {

        List<Order> orders = new();
        //Add booking
        CreateBooking(order, "tenant_Spike");
        var results = GetOrder("tenant_Spike");

        return new JsonResult(results.Last());


    }

    private void CreateBooking(Order order, string databaseName)
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
    private Dictionary<string, string> customOrder(Order order)
    {
        Dictionary<string, string> customBooking = new Dictionary<string, string>
            {
                { "orderNumber", "" + order.OrderNumber },
                { "costCenter", order.CostCenter },
                { "supplier", order.Supplier },
                { "shipTo", order.ShipTo },
                { "orderDate", "" + order.OrderDate },
                { "orderAmount", "" + order.OrderAmount },
                { "deliveryPeriod", "" + order.DeliveryPeriod},
                { "firstDeliveryDate", "" + order.FirstDeliveryDate },
                { "vehicleDetails", "" + order.VehicleDetails}
            };

        return customBooking;
    }

    public class Order
    {
        public int OrderNumber { get; set; }
        public string CostCenter { get; set; } = string.Empty;
        public string Supplier { get; set; } = string.Empty;
        public string ShipTo { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public Decimal OrderAmount { get; set; }
        public int DeliveryPeriod { get; set; }
        public DateTime FirstDeliveryDate { get; set; }
        public string VehicleDetails { get; set; } = string.Empty;
    }
}
