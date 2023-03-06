using Saas.Permissions.Service.Data.Context;

namespace Saas.Permissions.Service.Data;

public static class SaasPermissionDbInitializer
{
    public static void ConfigureDatabase(this IHost host)
    {
        using IServiceScope scope = host.Services.CreateScope();

        ILogger logger = scope.ServiceProvider.GetRequiredService<ILogger<SaasPermissionsContext>>();
        SaasPermissionsContext saasPermissionsContext = scope.ServiceProvider.GetRequiredService<SaasPermissionsContext>();

        CreateDatabase(saasPermissionsContext, logger);
        SeedDatabase(saasPermissionsContext, logger);
    }

    private static void CreateDatabase(SaasPermissionsContext context, ILogger logger)
    {
        try
        {
            context.Database.EnsureCreated();
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Unable to create the database");
            throw;
        }
    }

    private static void SeedDatabase(SaasPermissionsContext context, ILogger logger)
    {
        try
        {
            if (context.SaasPermissions.Any())
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
