using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BasicMessageStore.Configuration;
using BasicMessageStore.Models.Security;
using BasicMessageStore.Models.User;
using Microsoft.IdentityModel.Tokens;

namespace BasicMessageStore.Security
{
    /// <summary>
    /// Naive implementation of a JWT token provider, proper authentication through a server
    /// should be implemented. 
    /// </summary>
    public class JWTTokenProvider : ITokenProvider
    {
        private readonly IConfigurationProvider _configurationProvider;

        public JWTTokenProvider(IConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        public string GenerateAccessToken(User user)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configurationProvider.TokenSecret));
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username)
            };
            
            var token = new JwtSecurityToken(
                _configurationProvider.TokenIssuer,
                _configurationProvider.TokenAudience,
                claims,
                DateTime.Now,
                DateTime.Now.AddMinutes(_configurationProvider.TokenExpirationMinutes),
                new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256)
            );
            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}