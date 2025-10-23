using Microsoft.AspNetCore.Identity;
using Recipy.Models;

namespace Recipy.Services;

public class AuthService()
{
    private readonly PasswordHasher<User> _passwordHasher = new();

    public string HashPassword(User user, string plainPassword)
    {
        return _passwordHasher.HashPassword(user, plainPassword);
    }

    public bool Authenticate(User user, string plainPassword)
    {
        return _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, plainPassword) != PasswordVerificationResult.Failed;
    }
}