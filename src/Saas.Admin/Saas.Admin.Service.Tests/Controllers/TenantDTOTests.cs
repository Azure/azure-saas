namespace Saas.Admin.Service.Tests;

public class TenantDTOTests
{

    [Theory, AutoDataNSubstitute]
    public void All_Values_Are_Copied_To_Tenant(TenantDTO dto, byte[] versionBytes)
    {
        //Version needs to be a valid base 64 string
        dto.Version = Convert.ToBase64String(versionBytes);
        Tenant tenant = dto.ToTenant();

        AssertAdditions.AllPropertiesAreEqual(tenant, dto, nameof(tenant.ConcurrencyToken), nameof(tenant.CreatedTime));
    }

    [Theory, AutoDataNSubstitute]
    public void All_Values_Are_Copied_To_DTO(Tenant tenant)
    {
        TenantDTO dto = new TenantDTO(tenant);

        AssertAdditions.AllPropertiesAreEqual(dto, tenant, nameof(dto.Version), nameof(dto.CreatedTime));
        Assert.NotNull(tenant.CreatedTime);
        
        if (tenant.CreatedTime != null)
        {
            Assert.Equal(tenant.CreatedTime.Value, dto.CreatedTime);
        }
    }

    [Theory, AutoDataNSubstitute]
    public void Version_Is_Transferred_Correctly(TenantDTO dto, byte[] versionBytes)
    {
        //Version needs to be a valid base 64 string
        dto.Version = Convert.ToBase64String(versionBytes);
        Tenant tenant = dto.ToTenant();

        Assert.Equal(versionBytes, tenant.ConcurrencyToken);
    }

    [Theory, AutoDataNSubstitute]
    public void Over_Rides_Correct_Values_On_Copy(Tenant fromDb, TenantDTO fromUser)
    {
        fromUser.Id = fromDb.Id;
        fromUser.Version = fromUser.Version != null ? Convert.ToBase64String(Encoding.UTF8.GetBytes(fromUser.Version)) : null;

        Assert.NotEqual(fromDb.Name, fromUser.Name);

        fromUser.CopyTo(fromDb);

        Assert.Null(fromDb.CreatedTime);
        AssertAdditions.AllPropertiesAreEqual(fromDb, fromUser, nameof(fromDb.ConcurrencyToken), nameof(fromDb.CreatedTime));
    }

    [Theory, AutoDataNSubstitute]
    public void ConcurrencyToken_Is_Transfereed_Correctly(Tenant tenant)
    {
        TenantDTO dto = new TenantDTO(tenant);
        Assert.Equal(Convert.ToBase64String(tenant.ConcurrencyToken ?? Array.Empty<byte>()), dto.Version);
    }

    [Theory, AutoDataNSubstitute]
    public void Round_Trip_Concurrency_Token(Tenant tenant)
    {
        TenantDTO dto = new TenantDTO(tenant);
        Tenant converted = dto.ToTenant();

        Assert.NotSame(tenant, converted);
        Assert.NotSame(tenant.ConcurrencyToken, converted.ConcurrencyToken);
        Assert.Equal(tenant.ConcurrencyToken, converted.ConcurrencyToken);
    }

    [Theory, AutoDataNSubstitute]
    public void User_Cant_Override_Created_Date(Tenant tenant, DateTime original, DateTime updated)
    {
        Assert.NotEqual(original, updated);

        tenant.CreatedTime = original;

        TenantDTO dto = new TenantDTO(tenant);
        dto.CreatedTime = updated;

        Assert.NotEqual(tenant.CreatedTime, dto.CreatedTime);

        dto.CopyTo(tenant);
        Assert.Null(tenant.CreatedTime);
    }
}
