using AuthLogic;
using Dapper;
using Microsoft.AspNetCore.Identity.Data;

namespace AuthService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton<IDbConnectionFactory>(_ =>
                new NpgsqlConnectionFactory(builder.Configuration.GetConnectionString("DefaultConnection")!));
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            var app = builder.Build();

            using var connection = app.Services.GetRequiredService<IDbConnectionFactory>().CreateConnection();
            await connection.ExecuteAsync(@"
                CREATE TABLE IF NOT EXISTS identity_users (
                    id UUID PRIMARY KEY,
                    login VARCHAR(255) NOT NULL UNIQUE,
                    password_hash VARCHAR(255) NOT NULL,
                    email VARCHAR(255),
                    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
                    refresh_token VARCHAR(500),
                    refresh_token_expiry_time TIMESTAMPTZ
                );
            ");

            //app.MapPost("/register", async (RegisterRequest request, IAuthService authService) =>
            //{
            //    var result = await authService.RegisterAsync(request.Email, request.Password);
            //    return result.Succeeded ? Results.Ok() : Results.BadRequest(result.Errors);
            //});

            //app.MapPost("/login", async (LoginRequest request, IAuthService authService) =>
            //{
            //    var token = await authService.LoginAsync(request.Email, request.Password);
            //    return token is not null ? Results.Ok(new { Token = token }) : Results.Unauthorized();
            //});

            app.Run();
        }
    }
}
