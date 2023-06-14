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

    public TenantService(TenantsContext tenantContext, IPermissionsServiceClient permissionService, ILogger<TenantService> logger)
    {
        _context = tenantContext;
        _permissionService = permissionService;
        _logger = logger;
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
            Task task = ProvisionTenantDBAsync(tenant.Route, tenant);
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
        var tenant = await _context.Tenants.FirstOrDefaultAsync(x =>
            route != null
            && x.Route.Length == route.Length
            && EF.Functions.Like(x.Route, $"%{route}%"));
        TenantInfoDTO returnValue = new(tenant);
        return returnValue;
    }

    public async Task<bool> CheckPathExists(string path)
    {
        try
        {
            bool exists = await _context.Tenants.AnyAsync(t => string.Equals(t.Route, path));
            return exists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while checking for valid path");
            throw;
        }
    }

    private async Task ProvisionTenantDBAsync(string dbname, Tenant tenant)
    {
        int responseId = 0;

        using (SqlConnection con = new SqlConnection(_context.Database.GetConnectionString()))
        {
            if (con.State != ConnectionState.Open)
            {
                await con.OpenAsync();
            }
            using (SqlCommand commad = new SqlCommand("spCreateTenantDatabase", con))
            {
                commad.CommandType = CommandType.StoredProcedure;

                commad.Parameters.AddWithValue("databaseName", SqlDbType.NVarChar).Value = dbname;
                commad.Parameters.AddWithValue("tenantId", SqlDbType.UniqueIdentifier).Value = tenant.Guid;

                commad.CommandTimeout = 120;

                using (SqlDataReader reader = await commad.ExecuteReaderAsync())
                {
                    reader.Read();

                    //Get return value
                    responseId = reader.GetInt32(0);

                    if (responseId == 1)
                    {
                        dbname = reader.GetString(1);
                        _logger.LogInformation("Done DB initialization");
                    }
                }

                if(responseId == 1)//Update tenant id
                {
                    commad.CommandText = "spUpateTenantDatabase";
                    commad.Parameters.Clear();
                    commad.Parameters.AddWithValue("databaseName", SqlDbType.NVarChar).Value = dbname;
                    commad.Parameters.AddWithValue("tenantId", SqlDbType.UniqueIdentifier).Value = tenant.Guid;
                    using (SqlDataReader reader = await commad.ExecuteReaderAsync())
                    {
                        reader.Read();

                        //Get return value
                        int res = reader.GetInt32(0);

                        if (res == 1)
                        {
                            _logger.LogInformation("Set up complete");
                        }
                    }
                }
                else
                {
                    throw new Exception($"You {tenant.Company} of route {tenant.Route} database could not be provision");
                }
                
            }

           await con.CloseAsync();

        }
       
    }
}
