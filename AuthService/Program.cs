using AuthLogic;
using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

            builder.Services.AddScoped<IAuthServiceLogic, AuthServiceLogic>();
            builder.Services.AddScoped<ITokenGenerator, JwtTokenGenerator>();
            
            builder.Services.AddScoped<IPasswordHasher<AuthLogic.Models.IdentityUser>, PasswordHasher<AuthLogic.Models.IdentityUser>>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
                    };
                });

            builder.Services.AddAuthorization();

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

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapAuthEndpoints();

            app.Run();
        }
    }
}
