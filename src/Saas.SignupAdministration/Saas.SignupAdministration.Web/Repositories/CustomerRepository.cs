using Microsoft.Extensions.Configuration;
using Saas.SignupAdministration.Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Saas.SignupAdministration.Web.Repositories
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
                var customers = new List<Customer>();

                using (var connection = new SqlConnection(_configuration[SR.CatalogDbConnectionProperty]))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(SR.CatalogCustomerSelectQuery, connection))
                    {
                        var param = new SqlParameter();

                        param.ParameterName = SR.CatalogTenantIdParameter;
                        param.Value = tenantId;
                        command.Parameters.Add(param);

                        var reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            var customer = new Customer();

                            customer.Id = Guid.Parse(reader[SR.CatalogIdProperty].ToString());
                            customer.TenantId = Guid.Parse(reader[SR.CatalogTenantIdProperty].ToString());
                            customer.CustomerName = reader[SR.CatalogCustomerNameProperty].ToString();
                            customer.IsActive = bool.Parse(reader[SR.CatalogIsActiveProperty].ToString());
                            customers.Add(customer);
                        }

                        if (!reader.IsClosed)
                        {
                            await reader.CloseAsync();
                        }
                    }

                    if (connection.State != ConnectionState.Closed)
                    {
                        await connection.CloseAsync();
                    }

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
