using System;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Saas.AspNetCore.Authorization.ClaimTransformers
{
    public static class ClaimToRoleTransformerExtensions
    {
        public static IServiceCollection AddClaimToRoleTransformer(this IServiceCollection services, string sourceClaimType,
            string roleClaimType = ClaimToRoleTransformerOptions.DefaultRoleClaimType,
            string authenticationType = ClaimToRoleTransformerOptions.DefaultAuthenticationType)
        {
            services.AddOptions<ClaimToRoleTransformerOptions>()
                .Configure(options =>
                {
                    options.RoleClaimType = roleClaimType;
                    options.SourceClaimType = sourceClaimType;
                    options.AuthenticationType = authenticationType;
                });

            services.RegisterTransformer();
            return services;
        }

        public static IServiceCollection AddClaimToRoleTransformer(this IServiceCollection services,
            IConfiguration configurationSection, Action<ClaimToRoleTransformerOptions> configure = null)
        {
            services.Configure<ClaimToRoleTransformerOptions>(configurationSection);

            if (configure != null)
            {
                services.Configure<ClaimToRoleTransformerOptions>(configure);
            }

            services.RegisterTransformer();
            return services;
        }

        public static IServiceCollection AddClaimToRoleTransformer(this IServiceCollection services,
                    IConfiguration configuration, string configSectionName,
                    Action<ClaimToRoleTransformerOptions> configure = null)
        {
            if (configuration == null)
            {
                throw new ArgumentException("configuration");
            }

            if (string.IsNullOrEmpty(configSectionName))
            {
                throw new ArgumentException("configSectionName");
            }

            IConfigurationSection section = configuration.GetSection(configSectionName);

            AddClaimToRoleTransformer(services, section, configure);
            return services;
        }

        private static void RegisterTransformer(this IServiceCollection services)
        {
            services.AddTransient<IClaimsTransformation, ClaimToRoleTransformer>();
        }
    }
}
