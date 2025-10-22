namespace Recipy;
public class Program {

    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer(); // Swagger
        builder.Services.AddSwaggerGen();           // Swagger

        builder.Services.AddControllers();

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

        if (app.Environment.IsDevelopment()) {
            app.UseSwagger();    // Swagger
            app.UseSwaggerUI();  // Swagger

            app.UseDeveloperExceptionPage();
            app.UseCors("DevCors");
        } else {
            app.UseHttpsRedirection();
            app.UseCors("ProdCors");
        }

        app.MapControllers();
        app.Run();
    }
}
