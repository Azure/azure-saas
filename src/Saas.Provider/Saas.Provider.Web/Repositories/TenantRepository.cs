using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Saas.Provider.Web.Repositories
{
    public class TenantRepository
    {
        private readonly IConfiguration _configuration;

        public TenantRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> GetTenantId(Guid apiKey)
        {
            string tenantId = null;
            try
            {
                using (var connection = new SqlConnection(_configuration["ConnectionStrings:CatalogDbConnection"]))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("SELECT Id  FROM Tenant WHERE ApiKey = @apiKey", connection))
                    {
                        command.Parameters.AddWithValue("@apiKey", apiKey);
                        var reader = await command.ExecuteReaderAsync();
                        if (reader.Read())
                        {
                            tenantId = reader["Id"].ToString();
                        }

                        if (!reader.IsClosed) await reader.CloseAsync();
                        if (connection.State != ConnectionState.Closed) await connection.CloseAsync();

                        return tenantId;
                    }
                }
            }
            catch
            {
                return null;
            }
        }
    }
}