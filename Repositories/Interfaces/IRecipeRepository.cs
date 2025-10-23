using Recipy.Models;
namespace Recipy.Repositories.Interfaces;
public interface IRecipeRepository
{
    Task<List<Recipe>> GetAllAsync();
    Task AddAsync(Recipe recipe);
    Task SaveChangesAsync();
}