using Microsoft.AspNetCore.Mvc;
using Recipy.Dto.Users;
using Recipy.Models;
using Recipy.Repositories.Interfaces;
using Recipy.Services;

namespace Recipy.Controller;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IConfiguration config, IUserRepository userRepo) : ControllerBase
{
    private readonly IConfiguration _config = config;
    private readonly IUserRepository _userRepo = userRepo;
    private readonly AuthService _authService = new();
    private readonly JwtService _jwtService = new(config);

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
        user.PasswordHash = _authService.HashPassword(user, userRegisterDto.PlainPassword);

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

        if (_authService.Authenticate(user, userLoginDto.PlainPassword))
        {
            return Unauthorized("Incorrect Password");
        }
        else
        {
            return Ok(_jwtService.GenerateToken(user));
        }
    }
}