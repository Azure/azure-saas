namespace Saas.Admin.Service.Data;

public static class TenantDbInitializer
{

    public static void ConfigureDatabase(this IHost host)
    {
        using IServiceScope scope = host.Services.CreateScope();
        
        ILogger logger = scope.ServiceProvider.GetRequiredService<ILogger<TenantsContext>>();
        TenantsContext tenantsContext = scope.ServiceProvider.GetRequiredService<TenantsContext>();

        CreateDatabase(tenantsContext, logger);
        SeedDatabase(tenantsContext, logger);
    }

    private static void CreateDatabase(TenantsContext tenantsContext, ILogger logger)
    {
        try
        {
            tenantsContext.Database.EnsureCreated();
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Unable to create the database");
            throw;
        }
    }

    private static void SeedDatabase(TenantsContext tenantsContext, ILogger logger)
    {
        try
        {
            if (tenantsContext.Tenants.Any())
            {
                return;   // DB has been seeded
            }

            //Add any code required to seed the database here
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Error while seeding the database");
            throw;
        }
    }
}
