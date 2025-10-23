using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Recipy.Models;

namespace Recipy.Services;

public class JwtService(IConfiguration config)
{
    private readonly IConfiguration _config = config;
    public string GenerateToken(User user)
    {
        string jwtSecret = _config["Jwt:Secret"] ?? "THIS_IS_A_VERY_STRONG_SECRET_KEY";
        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(jwtSecret));
        SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

        Claim[] claims = [
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
            new Claim("email", user.Email)
        ];

        JwtSecurityToken token = new(
            issuer: null,
            audience: null,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}