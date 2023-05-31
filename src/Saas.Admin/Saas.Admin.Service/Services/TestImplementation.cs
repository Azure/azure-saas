using Saas.Shared.Options;

namespace Saas.Admin.Service.Services;

public class TestImplementation : ITest
{
    private readonly IConfiguration _configuration;

    public TestImplementation(IConfiguration config)
    {
        _configuration = config;
    }
    public string GetTestString()
    {
        var abc = _configuration.AsEnumerable();
        string all = "";
        foreach (var item in abc)
        {
            all += $"{item.Key} => {item.Value}\n";
        }

        return all;

        return _configuration.GetSection("https://kv-asdk-dev-lsg5.vault.azure.net/secrets/josetest").Get<string>() ?? "not given";
        SqlOptions? options = _configuration.GetRequiredSection(SqlOptions.SectionName).Get<SqlOptions>();

        if (options != null)
        {
            string alloptions = "";

            alloptions += $"admin login : {options.SQLAdministratorLoginName}\n";
            alloptions += $"permission login: {options.PermissionsSQLConnectionString}\n";
            alloptions += $"Tenant login : {options.TenantSQLConnectionString}\n";
            
            return alloptions;


        }else  return "sample";
    }
}
