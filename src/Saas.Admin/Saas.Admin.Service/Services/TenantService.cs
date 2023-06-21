using Microsoft.Data.SqlClient;
using Saas.Admin.Service.Controllers;
using Saas.Admin.Service.Data;
using Saas.Admin.Service.Data.Models.OnBoarding;
using Saas.Permissions.Client;
using System.Data;

namespace Saas.Admin.Service.Services;

public class TenantService : ITenantService
{
    private readonly TenantsContext _context;
    private readonly IPermissionsServiceClient _permissionService;
    private readonly ILogger _logger;
    private readonly IConfiguration _config;

    public TenantService(TenantsContext tenantContext, IPermissionsServiceClient permissionService, ILogger<TenantService> logger, IConfiguration config)
    {
        _context = tenantContext;
        _permissionService = permissionService;
        _logger = logger;
        _config = config;
    }

    public async Task<IEnumerable<TenantDTO>> GetAllTenantsAsync()
    {
        List<Tenant> allTenants = await _context.Tenants.ToListAsync();
        IEnumerable<TenantDTO> returnList = allTenants.Select(t => new TenantDTO(t));
        return returnList;
    }

    public async Task<TenantDTO> GetTenantAsync(Guid tenantId)
    {
        Tenant? tenant = await _context.Tenants.FindAsync(tenantId)
            ?? throw new ItemNotFoundExcepton("Tenant");

        TenantDTO returnValue = new(tenant);
        return returnValue;
    }

    public async Task<IEnumerable<TenantDTO>> GetTenantsByIdAsync(IEnumerable<Guid> ids)
    {
        IQueryable<Tenant>? tenants = _context.Tenants.Where(t => ids.Contains(t.Guid));

        List<TenantDTO>? returnValue = await tenants.Select(t => new TenantDTO(t)).ToListAsync();
        return returnValue;
    }

    public async Task<TenantDTO> AddUserToTenantAsync(NewTenantRequest newTenantRequest, Guid userId)
    {
        UserInfo? user = await _context.UserInfo.FindAsync(userId);
        if (user == null)
        {
            _context.UserInfo.Add(newTenantRequest.UserInfo);
        }

        UserTenant userTenant = newTenantRequest.UserTenant;
        _context.UserTenants.Add(userTenant);

        try
        {
            await _context.SaveChangesAsync();
            await _permissionService.AddNewTenantAsync(userTenant.TenantId, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting permission for tenant {tenantName}", newTenantRequest.Name);
            _context.UserTenants.Remove(userTenant);
            await _context.SaveChangesAsync();
            throw;
        }

        TenantDTO? returnValue = new(newTenantRequest.ToTenant());
        return returnValue;
    }

    public async Task<TenantDTO> AddTenantAsync(NewTenantRequest newTenantRequest, Guid adminId)
    {
        Tenant tenant = newTenantRequest.ToTenant();

        UserInfo? principalUser = await _context.UserInfo.FindAsync(adminId);
        if (principalUser == null)
        {
            _context.UserInfo.Add(newTenantRequest.UserInfo);
        }
        _context.Tenants.Add(tenant);

        UserTenant userTenant = newTenantRequest.UserTenant;
        //use the database generated Guid for tenant
        userTenant.CreatedUser = newTenantRequest.UserInfo.UserName;
        userTenant.PrincipalUser = true;
        userTenant.TenantId = tenant.Guid;
        _context.UserTenants.Add(userTenant);

        await _context.SaveChangesAsync();

        try
        {
            await _permissionService.AddNewTenantAsync(tenant.Guid, adminId);

            //Complete with db provision. Using a task.
            Task task = SetUpTenantDBAsync(tenant);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting permission for tenant {tenantName}", newTenantRequest.Name);
            _context.Tenants.Remove(tenant);
            _context.UserInfo.Remove(newTenantRequest.UserInfo);
            _context.UserTenants.Remove(userTenant);
            await _context.SaveChangesAsync();
            throw;
        }

        TenantDTO? returnValue = new(tenant);
        return returnValue;
    }

    public async Task<TenantDTO> UpdateTenantAsync(TenantDTO tenantDto)
    {
        Tenant? fromDB = await _context.Tenants.FindAsync(tenantDto.Id)
            ?? throw new ItemNotFoundExcepton("Tenant");

        tenantDto.CopyTo(fromDB);
        _context.Entry(fromDB).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogInformation(ex, "Concurrency exception saving changes to {TenatnID}, {TenantName}", fromDB.Id, fromDB.Company);
            throw;
        }

        TenantDTO returnValue = new(fromDB);
        return returnValue;
    }

    public async Task DeleteTenantAsync(Guid tenantId)
    {
        Tenant? tenant = await _context.Tenants.FindAsync(tenantId)
            ?? throw new ItemNotFoundExcepton("Tenant");

        _context.Tenants.Remove(tenant);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> TenantExistsAsync(Guid tenantId)
    {
        return await _context.Tenants.AnyAsync(e => e.Guid == tenantId);
    }

    public async Task<TenantInfoDTO> GetTenantInfoByRouteAsync(string route)
    {
        throw new NotImplementedException();
        //var tenant = await _context.Tenants.FirstOrDefaultAsync(x =>
        //    route != null
        //    && x.Route.Length == route.Length
        //    && EF.Functions.Like(x.Route, $"%{route}%"));
        //TenantInfoDTO returnValue = new(tenant);
        //return returnValue;
    }

    public async Task<bool> CheckPathExists(string path)
    {
        throw new NotImplementedException();
        //try
        //{
        //    bool exists = await _context.Tenants.AnyAsync(t => string.Equals(t.Route, path));
        //    return exists;
        //}
        //catch (Exception ex)
        //{
        //    _logger.LogError(ex, "Error while checking for valid path");
        //    throw;
        //}
    }


    /// <summary>
    /// Used to map a user tenant to an SQL server in specific region
    /// Called after a tenant is onboarded. that is, their information is captured to catalog
    /// Once done, it calls provision user database for
    /// Given it's intesity it should not be waited
    /// </summary>
    /// <param name="tenant"></param>
    /// <returns></returns>
    private async Task SetUpTenantDBAsync(Tenant tenant)
    {
        string connectionString = _context.Database.GetConnectionString()??string.Empty;
        try
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                if (con.State != ConnectionState.Open)
                {
                    await con.OpenAsync();
                }

                using (SqlCommand commad = new SqlCommand("spSetUpTenanatDb", con))
                {
                    commad.CommandType = CommandType.StoredProcedure;

                    commad.Parameters.AddWithValue("timezone", SqlDbType.NVarChar).Value = tenant.TimeZone;

                    using (SqlDataReader reader = await commad.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            tenant.SqlServerRegion = reader.GetString(0);
                            tenant.DatabaseName = reader.GetString(1);
                        }
                    }
                }
            }


            //Ensure that dbname and regions are set before exiting
            if (string.IsNullOrEmpty(tenant.DatabaseName) || string.IsNullOrEmpty(tenant.SqlServerRegion))
            {
                throw new Exception("Cannot provision a database when database name is empty or null");
            }

            //provision db then tenant initialization complete
            //if success provision user database
            bool result = await ProvisionTenantDBAsync(tenant);

            if (result)//Update tenant db initilization complete
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    if (con.State != ConnectionState.Open)
                    {
                        await con.OpenAsync();
                    }
                    using (SqlCommand commad = new SqlCommand("spUpateTenantDatabase", con))
                    {
                        commad.CommandType = CommandType.StoredProcedure;

                        commad.Parameters.AddWithValue("databaseName", SqlDbType.NVarChar).Value = tenant.DatabaseName;
                        commad.Parameters.AddWithValue("sqlregion", SqlDbType.NVarChar).Value = tenant.SqlServerRegion;
                        commad.Parameters.AddWithValue("tenantId", SqlDbType.UniqueIdentifier).Value = tenant.Guid;

                        using (SqlDataReader reader = await commad.ExecuteReaderAsync())
                        {
                            reader.Read();

                            //Get return value
                            int responseId = reader.GetInt32(0);

                            if (responseId == 1)
                            {
                                _logger.LogInformation("Done DB initialization");
                            }
                        }
                    }

                }
               
            }
        }
        catch(SqlException ex)
        {
            _logger.LogError("Error provision user database with error " + ex.Message);

        }catch(Exception ex)
        {
            _logger.LogError("Error provision user database with error " + ex.Message);
        }

    }

    private async Task<bool> ProvisionTenantDBAsync(Tenant tenant)
    {
        int responseId = 0;

        //Get Region connection string
        string regionConString = _config.GetSection("RegionsConStrings")?.Get<Dictionary<string, string>>()?[tenant.SqlServerRegion??""]??string.Empty;
        if (string.IsNullOrEmpty(regionConString))
        {
            throw new ArgumentNullException("Region cannot be null");
        }

        using (SqlConnection con = new SqlConnection(regionConString))
        {
            if (con.State != ConnectionState.Open)
            {
                await con.OpenAsync();
            }
            using (SqlCommand commad = new SqlCommand("spCreateTenantDatabase", con))
            {
                commad.CommandType = CommandType.StoredProcedure;

                commad.Parameters.AddWithValue("databaseName", SqlDbType.NVarChar).Value = tenant.DatabaseName;

                commad.CommandTimeout = 120;

                using (SqlDataReader reader = await commad.ExecuteReaderAsync())
                {
                    reader.Read();

                    //Get return value
                    responseId = reader.GetInt32(0);

                }

                if(responseId == 1)//Update tenant id
                {
                    _logger.LogInformation("Done DB initialization");
                    return true;
                }
                else
                {
                    throw new Exception($"You {tenant.Company} of database {tenant.DatabaseName} in the region{tenant.SqlServerRegion} could not be provision");
                }
                
            }

        }
       
    }
}
