using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Voicipher.Domain.Enums;
using Voicipher.Domain.Settings;
using Voicipher.Host.Utils;

namespace Voicipher.Host.Security.Extensions
{
    public static class VoicipherAuthorizationExtensions
    {
        public static IServiceCollection AddVoicipherAuthorization(this IServiceCollection services, AppSettings appSettings)
        {
            var issuerSigningKey = Encoding.ASCII.GetBytes(appSettings.SecretKey);

            services
                .AddAuthorization(options => { options.AddVoicipherPolicy(); })
                .AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = Constants.VoicipherScheme;
                    x.DefaultChallengeScheme = Constants.VoicipherScheme;
                })
                .AddJwtBearer(Constants.VoicipherScheme, x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(issuerSigningKey),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            return services;
        }

        public static AuthorizationOptions AddVoicipherPolicy(this AuthorizationOptions options)
        {
            var userPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(Constants.VoicipherScheme)
                .RequireAuthenticatedUser()
                .RequireRole(nameof(Role.User))
                .Build();
            var adminPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(Constants.VoicipherScheme)
                .RequireAuthenticatedUser()
                .RequireRole(nameof(Role.Admin))
                .Build();
            var securityPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(Constants.VoicipherScheme)
                .RequireAuthenticatedUser()
                .RequireRole(nameof(Role.Security))
                .Build();

            options.AddPolicy(nameof(VoicipherPolicy.User), userPolicy);
            options.AddPolicy(nameof(VoicipherPolicy.Admin), adminPolicy);
            options.AddPolicy(nameof(VoicipherPolicy.Security), securityPolicy);
            return options;
        }
    }
}
