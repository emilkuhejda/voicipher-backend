using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Voicipher.Domain.Settings;
using Voicipher.Host.Utils;

namespace Voicipher.Host.Security.Extensions
{
    public static class AzureAdExtensions
    {
        public static IServiceCollection AddAzureAdAuthorization(this IServiceCollection services, AppSettings appSettings)
        {
            services
                .AddAuthorization(options =>
                {
                    options.AddAzureAdPolicy();
                })
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = Constants.AzureAdScheme;
                    options.DefaultChallengeScheme = Constants.AzureAdScheme;
                })
                .AddJwtBearer(Constants.AzureAdScheme, options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.Audience = appSettings.Authentication.ClientId;
                    options.Authority = $"{appSettings.Authentication.AuthoritySignUpSignIn}/v2.0/";
                });

            return services;
        }

        public static AuthorizationOptions AddAzureAdPolicy(this AuthorizationOptions options)
        {
            var policy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(Constants.AzureAdScheme)
                .RequireAuthenticatedUser()
                .Build();

            options.AddPolicy(Constants.AzureAdPolicy, policy);
            return options;
        }
    }
}
