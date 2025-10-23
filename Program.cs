using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Recipy.Data;
using Recipy.Repositories;
using Recipy.Repositories.Interfaces;

namespace Recipy;
public class Program {

    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer(); // Swagger
        builder.Services.AddSwaggerGen();           // Swagger

        builder.Services.AddControllers();
        builder.Services.AddDbContext<RecipyContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("RecipyDb"))
        );
        builder.Services.AddScoped<IUserRepository, UserRepository>();

        // JWT & Auth
        string jwtSecret = builder.Configuration["Jwt:Secret"] ?? "THIS_IS_A_VERY_STRONG_SECRET_KEY";
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
            };
        });

        // Cors config
        builder.Services.AddCors((options) =>
        {
            options.AddPolicy("DevCors", (corsBuilder) =>
                {
                    corsBuilder.WithOrigins("http://localhost:4200", "http://localhost:3000", "http://localhost:8000")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            options.AddPolicy("ProdCors", (corsBuilder) =>
                {
                    corsBuilder.WithOrigins("https://myProductionSite.com")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
        });

        WebApplication app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();    // Swagger
            app.UseSwaggerUI();  // Swagger

            app.UseDeveloperExceptionPage();
            app.UseCors("DevCors");
        }
        else
        {
            app.UseHttpsRedirection();
            app.UseCors("ProdCors");
        }

        using (IServiceScope scope = app.Services.CreateScope())
        {
            RecipyContext context = scope.ServiceProvider.GetRequiredService<RecipyContext>();
            context.Database.OpenConnection();

            string migrationsFolder = Path.Combine(AppContext.BaseDirectory, "Database", "Migrations");
            IOrderedEnumerable<string> migrations = Directory.GetFiles(migrationsFolder, "*.sql").OrderBy(f => f);
            foreach (string m in migrations)
            {
                Console.WriteLine("Applying Migration: " + m);
                context.Database.ExecuteSqlRaw(File.ReadAllText(m));
            }
        }

        // JWT & Auth
        app.UseAuthentication();// Must be Before UseAuthorization
        app.UseAuthorization();

        app.MapControllers();
        app.Run();
    }
}
