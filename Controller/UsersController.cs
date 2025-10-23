using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Recipy.Data;
using Recipy.Dto;
using Recipy.Models;

namespace Recipy.Controller;

[ApiController]
[Route("[controller]")]
public class UsersController(RecipyContext context) : ControllerBase
{

    private readonly RecipyContext _context = context;
    private readonly PasswordHasher<User> _passwordHasher = new();

    // [HttpGet]
    // public IActionResult Test()
    // {
    //     return Ok(_context.Database.CanConnect());
    // }

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
        } else
        {
            return Ok();
        }
    }
}