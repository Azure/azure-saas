using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Saas.Shared.Options;
using System.Data;

namespace Saas.Shared.DataHandler;
public class DatabaseHandler : IDatabaseHandler
{
    private SqlConnection? _connection;
    private SqlCommand? command;

    private readonly List<Parameter> _parameters;

    private string? _connectionString;

    public List<Parameter> Parameters => _parameters;

    public int CommandTimeout { get; set; } = 30; //30 seconds as default

    public DatabaseHandler(IOptions<SqlOptions> sqloptions)
    {
        _parameters = new List<Parameter>();
        _connectionString = sqloptions.Value.IbizzSaasConnectionString;
    }

    public void CloseResources()
    {
        //clear parameters for further use
        CommandTimeout = 30;
        _parameters.Clear();
          command?.DisposeAsync();
        _connection?.DisposeAsync();
    }

    public async Task<SqlDataReader> ExecuteProcedureAsync(string procedureQuery, string? conString = null, CommandType cType = CommandType.StoredProcedure)
    {
        _connectionString = conString ?? _connectionString;
        try
        {
            _connection = new SqlConnection(_connectionString);
            {
                //Connect to database then read booking records
               if(_connection.State != ConnectionState.Open)
                {
                    await _connection.OpenAsync();
                }

                command = new SqlCommand(procedureQuery, _connection);
                {
                    command.CommandType = cType;
                    command.CommandTimeout = CommandTimeout;

                    foreach (Parameter parameter in _parameters)
                    {
                        switch (parameter.Type)
                        {
                            case SqlDbType.Int:
                                command.Parameters.AddWithValue(parameter.Name, parameter.Type).Value = int.Parse(parameter.Value ?? "-1");

                                break;

                            case SqlDbType.TinyInt:
                                command.Parameters.AddWithValue(parameter.Name, parameter.Type).Value = short.Parse(parameter.Value ?? "-1");
                                break;

                            case SqlDbType.BigInt:
                                command.Parameters.AddWithValue(parameter.Name, parameter.Type).Value = long.Parse(parameter.Value ?? "-1");
                                break;

                            case SqlDbType.Money:
                                command.Parameters.AddWithValue(parameter.Name, parameter.Type).Value = decimal.Parse(parameter.Value ?? "-1");
                                break;

                            default:
                                command.Parameters.AddWithValue(parameter.Name, parameter.Type).Value = parameter.Value;
                                break;
                        }

                    }

                    SqlDataReader reader = await command.ExecuteReaderAsync();

                    return reader;
                }
            }
        }
        catch (SqlException)
        {
            throw;
        }
        catch (Exception)
        {
            throw;
        }

    }

}
