using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
    public IActionResult GetRecipes()
    {
        return Ok(_context.Recipes.ToList());
    }

    [HttpPost("CreateRecipe")]
    [Authorize]
    public IActionResult CreateRecipe([FromBody] RecipeCreateDto recipeCreateDto)
    {
        Recipe recipe = new()
        {
            Title = recipeCreateDto.Title,
            Description = recipeCreateDto.Description,
            Ingredients = recipeCreateDto.Ingredients,
            Steps = recipeCreateDto.Steps,
            AuthorId = Guid.Parse(User.Claims.First().Value)
        };

        _context.Add(recipe);
        _context.SaveChanges();

        return Ok(recipe);
    }
}