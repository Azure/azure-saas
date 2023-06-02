using Microsoft.Data.SqlClient;
using Saas.Shared.Options;
using Saas.SignupAdministration.Web.Models;
using System.Data;

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

<<<<<<< Updated upstream
   
=======

>>>>>>> Stashed changes
    public async Task<SadUser> AddSadUser(SadUser sadUser, long userID)
    {
        try 
        {
            //Connect to database. then add user
            string? tenantCon = _sqlOptions?.TenantSQLConnectionString;

            if (tenantCon != null)
            {
                throw new NullReferenceException("SQL Connection string cannot be null.");
            }
            using (SqlConnection con = new SqlConnection(tenantCon))
            {
                await con.OpenAsync();

                using (SqlCommand command = new SqlCommand("spSaveVerifiedUsers", con))
                {
                    command.CommandType = CommandType.StoredProcedure;


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
        command.Parameters.AddWithValue("Answer", SqlDbType.NVarChar).Value = sadUser.Answer;
        command.Parameters.AddWithValue("Email", SqlDbType.NVarChar).Value = sadUser.Email;
        command.Parameters.AddWithValue("Telephone", SqlDbType.NVarChar).Value = sadUser.Telephone;

        //Add expiry date
        sadUser.ExpiryDate = DateOnly.FromDayNumber(90);


        command.Parameters.AddWithValue("ExpiryDate", SqlDbType.VarChar).Value = sadUser.ExpiryDate.ToShortDateString();
        
        //command.Parameters.AddWithValue("ExpiresAfter", SqlDbType.Int).Value = sadUser.ExpiresAfter;
        //command.Parameters.AddWithValue("LockAfter", SqlDbType.Int).Value = sadUser.LockAfter;
        //command.Parameters.AddWithValue("ImmediateChange", SqlDbType.Bit).Value = sadUser.ImmediateChange ? 1 : 0;
        //command.Parameters.AddWithValue("IsActive", SqlDbType.Bit).Value = sadUser.IsActive ? 1 : 0;
        //command.Parameters.AddWithValue("SuperUser", SqlDbType.Bit).Value = sadUser.SuperUser ? 1 : 0;
        //command.Parameters.AddWithValue("BioUserID", SqlDbType.NVarChar).Value = sadUser.BioUserID;
        //command.Parameters.AddWithValue("CCCode", SqlDbType.NVarChar).Value = sadUser.CCCode;
       
        command.Parameters.AddWithValue("RegSource", SqlDbType.NVarChar).Value = sadUser.RegSource;
        command.Parameters.AddWithValue("Narration", SqlDbType.NVarChar).Value = sadUser.Narration;
        
        command.Parameters.AddWithValue("DOB", SqlDbType.VarChar).Value = sadUser.DOB.ToShortDateString();
        
        //command.Parameters.AddWithValue("IDType", SqlDbType.NVarChar).Value = sadUser.IDType;
        //command.Parameters.AddWithValue("Profession", SqlDbType.NVarChar).Value = sadUser.Profession;
        //command.Parameters.AddWithValue("Company", SqlDbType.NVarChar).Value = sadUser.Company;
        //command.Parameters.AddWithValue("Employees", SqlDbType.Int).Value = sadUser.Employees;
        if(!string.IsNullOrEmpty(sadUser.Country))
            command.Parameters.AddWithValue("Country", SqlDbType.NVarChar).Value = sadUser.Country;

        //command.Parameters.AddWithValue("AcceptTerms", SqlDbType.Bit).Value = sadUser.AcceptTerms ? 1 : 0;
        //command.Parameters.AddWithValue("Notifications", SqlDbType.Bit).Value = sadUser.Notifications ? 1 : 0;
        //command.Parameters.AddWithValue("DBIdentity", SqlDbType.NVarChar).Value = sadUser.DBIdentity;
        //command.Parameters.AddWithValue("InitReady", SqlDbType.Bit).Value = sadUser.InitReady ? 1 : 0;
        //command.Parameters.AddWithValue("ExternalDB", SqlDbType.Bit).Value = sadUser.ExternalDB ? 1 : 0;
        command.Parameters.AddWithValue("PrincipalUser", SqlDbType.Bit).Value = sadUser.PrincipalUser ? 1 : 0;
        
        if(!string.IsNullOrEmpty(sadUser.TimeZone))
            command.Parameters.AddWithValue("TimeZone", SqlDbType.NVarChar).Value = sadUser.TimeZone;
        
        command.Parameters.AddWithValue("CreatedUser", SqlDbType.NVarChar).Value = sadUser.CreatedUser;
        command.Parameters.AddWithValue("CreatedDate", SqlDbType.VarChar).Value = sadUser.CreatedDate.ToShortDateString();
        command.Parameters.AddWithValue("UpdatedUser", SqlDbType.NVarChar).Value = sadUser.UpdatedUser;

        sadUser.UpdatedDate = DateOnly.FromDateTime(DateTime.Now);
        command.Parameters.AddWithValue("UpdatedDate", SqlDbType.VarChar).Value = sadUser.UpdatedDate.ToShortDateString();
        command.Parameters.AddWithValue("Terminus", SqlDbType.NVarChar).Value = sadUser.Terminus;
    }
}
