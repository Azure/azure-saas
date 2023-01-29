using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Saas.AspNetCore.Authorization.ClaimTransformers;

/// <summary>
/// Transforms a custom claim in space delimited format to roles
/// The user principal will factor in the custom roles when IsInRole is called
/// </summary>
public class ClaimToRoleTransformer : IClaimsTransformation
{
    private readonly string _sourceClaimType;
    private readonly string _roleClaimType;
    private readonly string _authenticationType;

    public ClaimToRoleTransformer(IOptions<ClaimToRoleTransformerOptions> claimToRoleTransformerOptions)
    {
        _sourceClaimType = claimToRoleTransformerOptions.Value.SourceClaimType
            ?? throw new NullReferenceException($"{nameof(claimToRoleTransformerOptions.Value.SourceClaimType)} cannot be null");

        _roleClaimType = claimToRoleTransformerOptions.Value.AuthenticationType
            ?? throw new NullReferenceException($"{nameof(claimToRoleTransformerOptions.Value.AuthenticationType)} cannot be null");

        _authenticationType = claimToRoleTransformerOptions.Value.RoleClaimType
            ?? throw new NullReferenceException($"{nameof(claimToRoleTransformerOptions.Value.RoleClaimType)} cannot be null");
    }

    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        System.Collections.Generic.IEnumerable<Claim> customClaims = principal.Claims
            .Where(c => _sourceClaimType
                .Equals(c.Type, StringComparison.OrdinalIgnoreCase));
        
        System.Collections.Generic.IEnumerable<Claim> roleClaims = customClaims
            .SelectMany(c =>
            {
                return c.Value.Split(' ').Select(s => 
                    new Claim(_roleClaimType, s));
            });

        if (!roleClaims.Any())
        {
            return Task.FromResult(principal);
        }

        ClaimsPrincipal transformed = new(principal);
        
        ClaimsIdentity rolesIdentity = new(
            roleClaims, 
            _authenticationType, 
            null, 
            _roleClaimType);
        
        transformed.AddIdentity(rolesIdentity);
        return Task.FromResult(transformed);
    }
}
