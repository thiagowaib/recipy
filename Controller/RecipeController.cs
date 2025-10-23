using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recipy.Data;
using Recipy.Dto;
using Recipy.Models;

namespace Recipy.Controller;

[ApiController]
[Route("api/[controller]")]
public class RecipeController(RecipyContext context) : ControllerBase
{
    private readonly RecipyContext _context = context;

    [HttpGet("GetRecipes")]
    public List<RecipeDto> GetRecipes()
    {
        List<RecipeDto> result = [];
        List<Recipe> recipes   = _context.Recipes.ToList();
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
    public IActionResult CreateRecipe([FromBody] RecipeDto recipeDto)
    {
        Recipe recipe = new()
        {
            Title = recipeDto.Title,
            Description = recipeDto.Description,
            Ingredients = recipeDto.Ingredients,
            Steps = recipeDto.Steps,
            AuthorId = Guid.Parse(User.Claims.First().Value)
        };

        _context.Add(recipe);
        _context.SaveChanges();

        return Ok(recipe);
    }
}