using Microsoft.EntityFrameworkCore;
using Recipy.Data;
using Recipy.Models;
using Recipy.Repositories.Interfaces;

namespace Recipy.Repositories;

public class UserRepository (RecipyContext context) : IUserRepository
{
    private readonly RecipyContext _context = context;
    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }


    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}