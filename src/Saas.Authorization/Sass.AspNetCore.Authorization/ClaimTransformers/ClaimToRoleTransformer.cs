using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sass.AspNetCore.Authorization.ClaimTransformers
{
/// <summary>
/// Transforms a custom claim in space delimited format to roles
/// The user pricipal will factor in the custom roles when IsInRole is called
/// </summary>
    public class ClaimToRoleTransformer : IClaimsTransformation
    {
        private readonly string _sourceClaimType;
        private readonly string _roleType;
        private readonly string _authType;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sourceClaimType">Name of the space delimited claim to transform</param>
        /// <param name="roleClaimType">Type of the individual role claims generated</param>
        /// <param name="authType">Authentication type to set the new identity to</param>
        public ClaimToRoleTransformer(string sourceClaimType, string roleClaimType, string authType)
        {
            _sourceClaimType = sourceClaimType;
            _roleType = roleClaimType;
            _authType = authType;
        }


        public ClaimToRoleTransformer(IOptions<ClaimToRoleTransformerOptions> options)
            : this(options.Value.SourceClaimType, options.Value.RoleClaimType, options.Value.AuthenticationType)
        { 
        }

        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var customClaims = principal.Claims.Where(c => _sourceClaimType.Equals(c.Type, StringComparison.OrdinalIgnoreCase));
            var roleClaims = customClaims.SelectMany(c =>
            {
                return c.Value.Split(' ').Select(s => new Claim(_roleType, s));
            });

            if (!roleClaims.Any())
            {
                return Task.FromResult(principal);
            }


            ClaimsPrincipal transformed = new ClaimsPrincipal(principal);
            ClaimsIdentity rolesIdentity = new ClaimsIdentity(roleClaims, _authType, null, _roleType);
            transformed.AddIdentity(rolesIdentity);
            return Task.FromResult(transformed);
        }
    }
}
