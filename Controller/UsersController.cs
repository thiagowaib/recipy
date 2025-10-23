using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Recipy.Data;
using Recipy.Dto.Users;
using Recipy.Models;
using Recipy.Repositories.Interfaces;

namespace Recipy.Controller;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IConfiguration config, IUserRepository userRepo) : ControllerBase
{
    private readonly IConfiguration _config = config;
    private readonly IUserRepository _userRepo = userRepo;
    private readonly PasswordHasher<User> _passwordHasher = new();

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDtro userRegisterDto)
    {
        if (await _userRepo.GetByEmailAsync(userRegisterDto.Email) != null)
        {
            return BadRequest("Email already being used");
        }
        else if(await _userRepo.GetByUsernameAsync(userRegisterDto.Username) != null)
        {
            return BadRequest("Username already being used");
        }

        User user = new()
        {
            Username = userRegisterDto.Username,
            Email = userRegisterDto.Email
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, userRegisterDto.PlainPassword);

        await _userRepo.AddAsync(user);
        await _userRepo.SaveChangesAsync();
        return Created();
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
    {
        User? user = await _userRepo.GetByEmailAsync(userLoginDto.Email);
        if(user == null)
        {
            user = await _userRepo.GetByUsernameAsync(userLoginDto.Username);
            if(user == null)
            {
                return BadRequest("User not found");
            }
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

            return Ok(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}