using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MisFinanzas.Application.Auth.Interfaces;
using MisFinanzas.Domain.Users;

namespace MisFinanzas.Infrastructure.Auth
{
    /// <summary>
    /// Genera tokens JWT firmados para los usuarios autenticados.
    /// </summary>
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly IConfiguration _config;

        public JwtTokenGenerator(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(ApplicationUser user)
        {
            // 1. Los "claims": la información que va DENTRO del token
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),           // quién es (el Id)
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),     // su email
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // id único del token
            };

            // 2. La clave secreta y el algoritmo de firma
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 3. Construir el token con emisor, audiencia, claims, expiración y firma
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    int.Parse(_config["Jwt:ExpiryMinutes"]!)),
                signingCredentials: credentials);

            // 4. Serializar el token a su forma de texto (el string que viaja)
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}