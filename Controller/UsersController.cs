using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Recipy.Data;
using Recipy.Dto;
using Recipy.Models;

namespace Recipy.Controller;

[ApiController]
[Route("[controller]")]
public class UsersController(RecipyContext context, IConfiguration config) : ControllerBase
{

    private readonly RecipyContext _context = context;
    private readonly IConfiguration _config = config;
    private readonly PasswordHasher<User> _passwordHasher = new();

    [HttpPost("Register")]
    public IActionResult Register([FromBody] UserRegisterDtro userRegisterDto)
    {
        if (_context.Users.Any(
            u => u.Username == userRegisterDto.Username
              || u.Email == userRegisterDto.Email
        ))
        {
            return BadRequest("Username or email already exists");
        }

        User user = new()
        {
            Username = userRegisterDto.Username,
            Email = userRegisterDto.Email
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, userRegisterDto.PlainPassword);

        _context.Users.Add(user);
        _context.SaveChanges();
        return Created();
    }

    [HttpPost("Login")]
    public IActionResult Login([FromBody] UserLoginDto userLoginDto)
    {
        User? user = _context.Users.Where(u => u.Username == userLoginDto.Username || u.Email == userLoginDto.Email).FirstOrDefault();

        if (user == null)
        {
            return BadRequest("User not found");
        }

        if (_passwordHasher.VerifyHashedPassword(user, user.PasswordHash, userLoginDto.PlainPassword) == PasswordVerificationResult.Failed)
        {
            return Unauthorized("Incorrect Password");
        }
        else
        {
            string jwtSecret = _config["Jwt:Secret"] ?? "THIS_IS_A_VERY_STRONG_SECRET_KEY";
            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(jwtSecret));
            SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

            Claim[] claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim("email", user.Email)
            };

            JwtSecurityToken token = new(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return Ok(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }

    [HttpGet("TestAuth")]
    [Authorize]
    public IActionResult TestAuth()
    {
        return Ok();
    }
}