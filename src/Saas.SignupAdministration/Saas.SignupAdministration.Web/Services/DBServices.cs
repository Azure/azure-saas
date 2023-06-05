
using Microsoft.Data.SqlClient;
using Saas.Shared.Options;
using Saas.SignupAdministration.Web.Interfaces;
using System.Data;

namespace Saas.SignupAdministration.Web.Services;

public class DBServices : IDBServices
{
    private readonly SqlOptions? _sqlOptions;

    public DBServices(SqlOptions sqlOptions)
    {
        _sqlOptions= sqlOptions;
    }
    public async Task<bool> isUserRegistered(string email)
    {
        try
        {
            string constr = _sqlOptions?.IbizzSaasConnectionString ?? string.Empty;
            //Connect to database. then add user
            using (SqlConnection con = new SqlConnection(constr))
            {
                await con.OpenAsync();

                int flag = 0;

                using (SqlCommand command = new SqlCommand("spCheckIfUserExists", con))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("UserName", SqlDbType.NVarChar).Value = email;



                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {

                        while (reader.Read())
                        {
                            flag = reader.GetInt32(0);
                        }

                    }

                }
                bool isRegistered = flag == 1 ? true : false;
                return isRegistered;

            }

        }
        catch (SqlException)
        {
            throw;
        }
    }
}
