using API_website.Core.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using API_website.Application.Interfaces.Auth;

namespace API_website.Infrastructure
{
    public class JwpProvider : IJwtProvider
    {
        private readonly JwtOptions _options;
        public JwpProvider(IOptions<JwtOptions> options)
        {
            _options = options.Value;
        }
        public string GnerateToken(User user)
        {
            Claim[] claims = {new ("userid", user.Id.ToString())};

            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                signingCredentials: signingCredentials,
                expires: DateTime.UtcNow.AddHours(_options.ExpiresHours)
                );

            var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenValue;
        }
    }
}
