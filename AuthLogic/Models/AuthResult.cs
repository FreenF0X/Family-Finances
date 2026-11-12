using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthLogic.Models
{
    public enum AuthError
    {
        None,
        InvalidCredentials,// неверный логин или пароль
        LoginAlreadyTaken,// логин занят при регистрации
        UserNotFound,
        InvalidRefreshToken,
        RefreshTokenExpired,
        SamePassword
    }

    public class AuthResult
    {
        public bool Succeeded { get; init; }
        public AuthError Error { get; init; }
        public string? AccessToken { get; init; }
        public string? RefreshToken { get; init; }
        public IdentityUser? User { get; init; }

        public static AuthResult Success(IdentityUser user, string accessToken, string refreshToken) =>
            new() { Succeeded = true, User = user, AccessToken = accessToken, RefreshToken = refreshToken };

        public static AuthResult Success(IdentityUser user) =>
            new() { Succeeded = true, User = user };

        public static AuthResult Fail(AuthError error) =>
            new() { Succeeded = false, Error = error };
    }
}
