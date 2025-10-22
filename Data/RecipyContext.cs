using Microsoft.EntityFrameworkCore;
using Recipy.Models;

namespace Recipy.Data;

public class RecipyContext(DbContextOptions<RecipyContext> options) : DbContext(options)
{
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<User> Users { get; set; }
}