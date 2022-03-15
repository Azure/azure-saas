namespace Saas.AspNetCore.Authorization.ClaimTransformers
{
    public class ClaimToRoleTransformerOptions
    {
        public const string ConfigSectionName = "ClaimToRoleTransformer";
        public const string DefaultRoleClaimType = "CustomRole";
        public const string DefaultAuthenticationType = "CustomRoleAuthentication";


        /// <summary>
        /// Name of the space delimited claim to transform
        /// </summary>
        public string SourceClaimType { get; set; } = string.Empty;

        /// <summary>
        /// Type of the individual role claims generated
        /// </summary>
        public string RoleClaimType { get; set; } = string.Empty;

        /// <summary>
        /// Authentication type to set the new identity to
        /// </summary>
        public string AuthenticationType { get; set; } = string.Empty;
    }
}
