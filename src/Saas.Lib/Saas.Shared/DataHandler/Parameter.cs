using System.Data;

namespace Saas.Shared.DataHandler;

//Holds a parameter value, name and sql type
public class Parameter
{
    public string? Name { get; set; }

    public string? Value { get; set; }

    public SqlDbType Type { get; set; }

}
