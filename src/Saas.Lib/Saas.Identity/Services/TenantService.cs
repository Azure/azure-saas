using Microsoft.Data.SqlClient;
using Saas.Shared.DataHandler;

using System.Data;

namespace Saas.Identity.Services;
public class CustomTenantService : ICustomTenantService
{
    private readonly IDatabaseHandler _dbHandler;

    public CustomTenantService(IDatabaseHandler dbHander)
    {
        _dbHandler = dbHander;
    }
    public async Task<Guid> GetActiveTenantAsync(Guid userId)
    {
        //Create a list of parameters
        List<Parameter> parameters = new()
        {
            new Parameter{Value= userId.ToString(), Name = "UserId", Type = SqlDbType.UniqueIdentifier},
            new Parameter{Value =""+ ICustomTenantService.GetActiveTenantFlag, Name = "Action", Type = SqlDbType.TinyInt}
        };

        _dbHandler.Parameters.AddRange(parameters);

        try
        {
            Guid tenantGuid = Guid.Empty;
            using(SqlDataReader reader = await _dbHandler.ExecuteProcedureAsync("spActiveTenantforUser"))
            {
                
                while (reader.Read())
                {
                    tenantGuid = reader.GetGuid(0);
                }
                await reader.CloseAsync();
            }

            return tenantGuid;
        }
        catch
        {
            throw;
        }
        finally
        {
            _dbHandler.CloseResources();
        }

    }

    public async Task<bool> UpdateActiveTenantAsync(Guid userId, Guid tenantId)
    {
        //Create a list of parameters
        List<Parameter> parameters = new()
        {
            new Parameter{Value= userId.ToString(), Name = "UserId", Type = SqlDbType.UniqueIdentifier},
            new Parameter{Value =""+ ICustomTenantService.UpdateActiveTenantFlag, Name = "Action", Type = SqlDbType.TinyInt},
            new Parameter{Value =tenantId.ToString(), Name = "TenantId", Type = SqlDbType.UniqueIdentifier}
        };

        _dbHandler.Parameters.AddRange(parameters);

        try
        {
            bool isSuccess = false;
            SqlDataReader reader = await _dbHandler.ExecuteProcedureAsync("spActiveTenantforUser");
            reader.Read();

            isSuccess = reader.GetInt32(0) == 1;

            await reader.CloseAsync();

            return isSuccess;
        }
        catch
        {
            throw;
        }
        finally
        {
            _dbHandler.CloseResources();
        }

    }
}
