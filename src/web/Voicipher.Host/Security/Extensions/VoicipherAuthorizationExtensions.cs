using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
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
                .AddAuthorization(options => { options.AddRewriteMePolicy(); })
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

        public static AuthorizationOptions AddRewriteMePolicy(this AuthorizationOptions options)
        {
            var policy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(Constants.VoicipherScheme)
                .RequireAuthenticatedUser()
                .Build();

            options.AddPolicy(Constants.VoicipherPolicy, policy);
            return options;
        }
    }
}
