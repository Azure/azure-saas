using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Saas.LandingSignup.Web.Repositories
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
                using (var connection = new SqlConnection(_configuration[SR.CatalogDbConnectionProperty]))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(SR.CatalogTenantSelectQuery, connection))
                    {
                        command.Parameters.AddWithValue(SR.CatalogApiKeyParameter, apiKey);

                        var reader = await command.ExecuteReaderAsync();

                        if (reader.Read())
                        {
                            tenantId = reader[SR.CatalogIdProperty].ToString();
                        }

                        if (!reader.IsClosed)
                        {
                            await reader.CloseAsync();
                        }

                        if (connection.State != ConnectionState.Closed)
                        {
                            await connection.CloseAsync();
                        }

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