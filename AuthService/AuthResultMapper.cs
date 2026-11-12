using AuthLogic.Models;
using AuthService.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService
{
    public static class AuthResultMapper
    {
        public static IResult ToHttpResult(this AuthResult result)
        {
            if (result.Succeeded)
            {
                return Results.Ok(new AuthResponse(
                    result.User!.Id,
                    result.User.Login,
                    result.AccessToken!,
                    result.RefreshToken!));
            }

            return result.Error switch
            {
                AuthError.InvalidCredentials => Results.Json(
                    new ErrorResponse("InvalidCredentials", "Неверный логин или пароль."),
                    statusCode: StatusCodes.Status401Unauthorized),

                AuthError.LoginAlreadyTaken => Results.Json(
                    new ErrorResponse("LoginAlreadyTaken", "Такой логин уже занят."),
                    statusCode: StatusCodes.Status409Conflict),

                AuthError.UserNotFound => Results.Json(
                    new ErrorResponse("UserNotFound", "Пользователь не найден."),
                    statusCode: StatusCodes.Status404NotFound),

                AuthError.InvalidRefreshToken => Results.Json(
                    new ErrorResponse("InvalidRefreshToken", "Недействительный refresh-токен."),
                    statusCode: StatusCodes.Status401Unauthorized),

                AuthError.RefreshTokenExpired => Results.Json(
                    new ErrorResponse("RefreshTokenExpired", "Refresh-токен истёк."),
                    statusCode: StatusCodes.Status401Unauthorized),

                AuthError.SamePassword => Results.Json(
                    new ErrorResponse("SamePassword", "Новый пароль совпадает со старым."),
                    statusCode: StatusCodes.Status400BadRequest),

                _ => Results.StatusCode(StatusCodes.Status500InternalServerError)
            };
        }
    }
}
