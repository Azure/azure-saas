using Microsoft.Data.SqlClient;
using System.Data;

namespace Saas.Shared.DataHandler;
public interface IDatabaseHandler
{
    public void CloseResources();

    public int CommandTimeout { get; set; }
    public List<Parameter> Parameters { get; }

    public Task<SqlDataReader> ExecuteProcedureAsync(string procedureQuery, string? conString = null, CommandType cType = CommandType.StoredProcedure);
}
