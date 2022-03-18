namespace Saas.Admin.Service.Tests;

public class TenantsContextTests
{

    [Theory, AutoDataNSubstitute]
    public async Task Add_New_Will_Update_Created_Time(TenantsContext context, Tenant tenant)
    {
        tenant.CreatedTime = null;

        await context.AddAsync(tenant);
        await context.SaveChangesAsync();

        Assert.NotNull(tenant.CreatedTime);
    }
}
