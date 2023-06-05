using Microsoft.Data.SqlClient;
using Saas.Shared.Options;
using Saas.SignupAdministration.Web.Models;
using System.Data;
using System.Security.Claims;

namespace Saas.Admin.Service.Services;

public class SadUserService : ISadUserService
{
    private readonly SqlOptions? _sqlOptions;
    private readonly string _hashSalt;

    public SadUserService(SqlOptions sqlOptions, string hashSalt)
    {
        _sqlOptions = sqlOptions;
        _hashSalt = hashSalt;
    }

    public async Task<SadUser> AddSadUser(SadUser sadUser, long userID)
    {
        try 
        {

            //Connect to database. then add user
            string? tenantCon = _sqlOptions?.IbizzSaasConnectionString;

            if (tenantCon == null)
            {
                throw new NullReferenceException("SQL Connection string cannot be null.");
            }
            using (SqlConnection con = new SqlConnection(tenantCon))
            {
                
                await con.OpenAsync();

                using (SqlCommand command = new SqlCommand("spSaveVerifiedUsers", con))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    sqlProcedureParams(command, sadUser);


                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {

                        while (reader.Read())
                        {
                            sadUser.Id = reader.GetInt64(0);
                        }

                    }

                }

            }

        }
        catch (SqlException)
        {
            throw;
        }
        catch(Exception)
        {
            throw;
        }

        return sadUser;
    }

    private void sqlProcedureParams(SqlCommand command, SadUser sadUser)
    {
        command.Parameters.AddWithValue("UserName", SqlDbType.NVarChar).Value = sadUser.UserName;
        command.Parameters.AddWithValue("FullNames", SqlDbType.NVarChar).Value = sadUser.FullNames;

        //pass employer number if only available
        if(!string.IsNullOrWhiteSpace(sadUser.EmpNo))
            command.Parameters.AddWithValue("EmpNo", SqlDbType.NVarChar).Value = sadUser.EmpNo;

        //hash password and confirm password
        command.Parameters.AddWithValue("Password", SqlDbType.NVarChar).Value = SadUser.passwordHash(sadUser.Password, _hashSalt);
        command.Parameters.AddWithValue("ConfirmPassword", SqlDbType.NVarChar).Value = SadUser.passwordHash(sadUser.ConfirmPassword, _hashSalt);


        command.Parameters.AddWithValue("Question", SqlDbType.NVarChar).Value = sadUser.Question;
        command.Parameters.AddWithValue("Answer", SqlDbType.NVarChar).Value = SadUser.passwordHash(sadUser.Answer, _hashSalt);
        command.Parameters.AddWithValue("Email", SqlDbType.NVarChar).Value = sadUser.Email ?? sadUser.UserName;
        command.Parameters.AddWithValue("Telephone", SqlDbType.NVarChar).Value = sadUser.Telephone;
       
        command.Parameters.AddWithValue("RegSource", SqlDbType.NVarChar).Value = sadUser.RegSource;

        if(!string.IsNullOrEmpty(sadUser.Narration))
            command.Parameters.AddWithValue("Narration", SqlDbType.NVarChar).Value = sadUser.Narration;
        
        command.Parameters.AddWithValue("DOB", SqlDbType.VarChar).Value = sadUser.DOB.ToShortDateString();
        
        if(!string.IsNullOrEmpty(sadUser.Profession))
            command.Parameters.AddWithValue("Profession", SqlDbType.NVarChar).Value = sadUser.Profession;
        
        if(!string.IsNullOrEmpty(sadUser.Company))
            command.Parameters.AddWithValue("Company", SqlDbType.NVarChar).Value = sadUser.Company;
        
        if(!string.IsNullOrEmpty(sadUser.Country))
            command.Parameters.AddWithValue("Country", SqlDbType.NVarChar).Value = sadUser.Country;

        //command.Parameters.AddWithValue("DBIdentity", SqlDbType.NVarChar).Value = sadUser.DBIdentity;
        
        command.Parameters.AddWithValue("PrincipalUser", SqlDbType.Bit).Value = sadUser.PrincipalUser ? 1 : 0;
        
        if(!string.IsNullOrEmpty(sadUser.TimeZone))
            command.Parameters.AddWithValue("TimeZone", SqlDbType.NVarChar).Value = sadUser.TimeZone;
        
        command.Parameters.AddWithValue("CreatedUser", SqlDbType.NVarChar).Value = sadUser.UserName;
        command.Parameters.AddWithValue("UpdatedUser", SqlDbType.NVarChar).Value = sadUser.UpdatedUser ?? sadUser.UserName;

        command.Parameters.AddWithValue("Terminus", SqlDbType.NVarChar).Value = sadUser.Terminus;
    }
}
