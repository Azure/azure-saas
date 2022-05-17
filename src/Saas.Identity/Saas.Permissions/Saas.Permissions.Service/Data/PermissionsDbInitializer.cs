namespace Saas.Permissions.Service.Data;

public static class PermissionsDbInitializer
{

    public static void ConfigureDatabase(this IHost host)
    {
        using IServiceScope scope = host.Services.CreateScope();

        ILogger logger = scope.ServiceProvider.GetRequiredService<ILogger<PermissionsContext>>();
        PermissionsContext permissionsContext = scope.ServiceProvider.GetRequiredService<PermissionsContext>();

        CreateDatabase(permissionsContext, logger);
        SeedDatabase(permissionsContext, logger);
    }

    private static void CreateDatabase(PermissionsContext permissionsContext, ILogger logger)
    {
        try
        {
            permissionsContext.Database.EnsureCreated();
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Unable to create the database");
            throw;
        }
    }

    private static void SeedDatabase(PermissionsContext permissionsContext, ILogger logger)
    {
        try
        {
            if (permissionsContext.Permissions.Any())
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
