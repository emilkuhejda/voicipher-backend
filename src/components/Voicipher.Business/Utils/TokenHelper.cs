using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Voicipher.Business.Utils
{
    public static class TokenHelper
    {
        public static string Generate(string secretKey, Claim[] claims, TimeSpan expireTime)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(expireTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }

        public static ClaimsPrincipal ValidateToken(string secretKey, string token)
        {
            var validator = new JwtSecurityTokenHandler();
            var issuerSigningKey = Encoding.ASCII.GetBytes(secretKey);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(issuerSigningKey),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            if (validator.CanReadToken(token))
            {
                try
                {
                    return validator.ValidateToken(token, validationParameters, out _);
                }
                catch (SecurityTokenException)
                {
                    return null;
                }
            }

            return null;
        }
    }
}
