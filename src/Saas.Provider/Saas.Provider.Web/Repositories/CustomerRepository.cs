using Microsoft.Extensions.Configuration;
using Saas.Provider.Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Saas.Provider.Web.Repositories
{
    public class CustomerRepository
    {
        private readonly IConfiguration _configuration;

        public CustomerRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<Customer>> GetAllCustomers(string tenantId)
        {
            try
            {
                List<Customer> customers = new List<Customer>();

                using (var connection = new SqlConnection(_configuration["ConnectionStrings:TenantDbConnection"]))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("SELECT * FROM dbo.Customer Where TenantId = @TenantId", connection))
                    {
                        SqlParameter param = new SqlParameter();
                        param.ParameterName = "@TenantId";
                        param.Value = tenantId;
                        command.Parameters.Add(param);

                        var reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            Customer customer = new Customer();
                            customer.Id = Guid.Parse(reader["Id"].ToString());
                            customer.TenantId = Guid.Parse(reader["TenantId"].ToString());
                            customer.CustomerName = reader["CustomerName"].ToString();
                            customer.IsActive = bool.Parse(reader["IsActive"].ToString());
                            customers.Add(customer);
                        }

                        if (!reader.IsClosed) await reader.CloseAsync();
                    }

                    if (connection.State != ConnectionState.Closed) await connection.CloseAsync();
                    return customers;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
