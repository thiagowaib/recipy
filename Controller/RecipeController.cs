using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recipy.Dto.Recipe;
using Recipy.Models;
using Recipy.Repositories.Interfaces;

namespace Recipy.Controller;

[ApiController]
[Route("api/[controller]")]
public class RecipeController(IRecipeRepository recipeRepo) : ControllerBase
{
    private readonly IRecipeRepository _recipeRrepo = recipeRepo;

    [HttpGet("GetRecipes")]
    public async Task<List<RecipeDto>> GetRecipes()
    {
        List<RecipeDto> result = [];
        List<Recipe> recipes   = await _recipeRrepo.GetAllAsync();
        foreach (Recipe recipe in recipes)
        {
            result.Add(new RecipeDto()
            {
                Title = recipe.Title,
                Description = recipe.Description,
                Ingredients = recipe.Ingredients,
                Steps = recipe.Steps,
            });
        }
        return result;
    }

    [HttpPost("CreateRecipe")]
    [Authorize]
    public async Task<IActionResult> CreateRecipe([FromBody] RecipeDto recipeDto)
    {
        Recipe recipe = new()
        {
            Title = recipeDto.Title,
            Description = recipeDto.Description,
            Ingredients = recipeDto.Ingredients,
            Steps = recipeDto.Steps,
            AuthorId = Guid.Parse(User.Claims.First().Value)
        };

        await _recipeRrepo.AddAsync(recipe);
        await _recipeRrepo.SaveChangesAsync();

        return Ok(recipe);
    }
}