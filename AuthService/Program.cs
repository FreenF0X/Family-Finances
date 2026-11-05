using AuthLogic;
using Microsoft.AspNetCore.Identity.Data;

namespace AuthService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton<IDbConnectionFactory>(_ =>
                new NpgsqlConnectionFactory(builder.Configuration.GetConnectionString("DefaultConnection")!));
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            var app = builder.Build();

            app.MapPost("/register", async (RegisterRequest request, IAuthService authService) =>
            {
                var result = await authService.RegisterAsync(request.Email, request.Password);
                return result.Succeeded ? Results.Ok() : Results.BadRequest(result.Errors);
            });

            app.MapPost("/login", async (LoginRequest request, IAuthService authService) =>
            {
                var token = await authService.LoginAsync(request.Email, request.Password);
                return token is not null ? Results.Ok(new { Token = token }) : Results.Unauthorized();
            });

            app.Run();
        }
    }
}
