using AuthLogic;
using AuthService.Contracts;

namespace AuthService
{
    public static class AuthEndpoints
    {
        public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/auth").WithTags("Auth");

            group.MapPost("/register", async (
                RegisterRequest request,
                IAuthServiceLogic authService) =>
            {
                var result = await authService.RegisterAsync(
                    request.Login, request.Password, request.Email);

                return result.ToHttpResult();
            });

            group.MapPost("/login", async (
                LoginRequest request,
                IAuthServiceLogic authService) =>
            {
                var result = await authService.LoginAsync(request.Login, request.Password);
                return result.ToHttpResult();
            });

            group.MapPost("/refresh", async (
                RefreshTokenRequest request,
                IAuthServiceLogic authService) =>
            {
                var result = await authService.RefreshTokenAsync(request.RefreshToken);
                return result.ToHttpResult();
            });

            group.MapPost("/change-password", async (
                ChangePasswordRequest request,
                IAuthServiceLogic authService,
                HttpContext httpContext) =>
            {
                // Достаём userId из JWT (claim Sub)
                var userIdClaim = httpContext.User.FindFirst("sub")?.Value
                               ?? httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (!Guid.TryParse(userIdClaim, out var userId))
                    return Results.Unauthorized();

                var result = await authService.ChangePasswordAsync(
                    userId, request.OldPassword, request.NewPassword);

                if (!result.Succeeded)
                    return result.ToHttpResult(); // вернёт ошибку, но не Ok

                // Успех — без тела
                return Results.NoContent();
            })
            .RequireAuthorization(); // смена пароля только для залогиненных

            group.MapPost("/logout", async (
                IAuthServiceLogic authService,
                HttpContext httpContext) =>
            {
                var userIdClaim = httpContext.User.FindFirst("sub")?.Value
                               ?? httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (!Guid.TryParse(userIdClaim, out var userId))
                    return Results.Unauthorized();

                await authService.LogoutAsync(userId);
                return Results.NoContent();
            })
            .RequireAuthorization();

            return app;
        }
    }
}
