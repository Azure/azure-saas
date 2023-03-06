namespace Saas.Identity.Authorization.Model.Data;

public interface IPermissionClaim
{
    string EntityIdentifier { get; }
    string ToClaim();
}
