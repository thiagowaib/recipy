using Microsoft.EntityFrameworkCore;
using Recipy.Data;
using Recipy.Models;
using Recipy.Repositories.Interfaces;

namespace Recipy.Repositories;

public class RecipeRepository (RecipyContext context) : IRecipeRepository
{
    private readonly RecipyContext _context = context;
    public async Task AddAsync(Recipe recipe)
    {
        await _context.Recipes.AddAsync(recipe);
    }

    public async Task<List<Recipe>> GetAllAsync()
    {
        return await _context.Recipes.ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}