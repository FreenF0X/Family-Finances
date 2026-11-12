using AuthLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AuthLogic
{
    public class AuthServiceLogic: IAuthServiceLogic
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<IdentityUser> _passwordHasher;
        private readonly ITokenGenerator _tokenGenerator;

        private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(7);

        public AuthServiceLogic(
            IUserRepository userRepository,
            IPasswordHasher<IdentityUser> passwordHasher,
            ITokenGenerator tokenGenerator)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<AuthResult> RegisterAsync(string login, string password, string? email)
        {
            var existing = await _userRepository.GetByLoginAsync(login);
            if (existing is not null)
                return AuthResult.Fail(AuthError.LoginAlreadyTaken);

            var user = new IdentityUser
            {
                Id = Guid.NewGuid(),
                Login = login,
                Email = email,
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            await _userRepository.CreateAsync(user);

            // Регистрация сразу логинит пользователя
            var accessToken = _tokenGenerator.GenerateAccessToken(user);
            var refreshToken = _tokenGenerator.GenerateRefreshToken();

            await _userRepository.UpdateRefreshTokenAsync(
                user.Id, refreshToken, DateTime.UtcNow.Add(RefreshTokenLifetime));

            return AuthResult.Success(user, accessToken, refreshToken);
        }

        public async Task<AuthResult> LoginAsync(string login, string password)
        {
            var user = await _userRepository.GetByLoginAsync(login);
            if (user is null)
                return AuthResult.Fail(AuthError.InvalidCredentials);

            var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (verification == PasswordVerificationResult.Failed)
                return AuthResult.Fail(AuthError.InvalidCredentials);

            var accessToken = _tokenGenerator.GenerateAccessToken(user);
            var refreshToken = _tokenGenerator.GenerateRefreshToken();

            await _userRepository.UpdateRefreshTokenAsync(
                user.Id, refreshToken, DateTime.UtcNow.Add(RefreshTokenLifetime));

            return AuthResult.Success(user, accessToken, refreshToken);
        }

        public async Task<AuthResult> RefreshTokenAsync(string refreshToken)
        {
            var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
            if (user is null)
                return AuthResult.Fail(AuthError.InvalidRefreshToken);

            if (user.RefreshTokenExpiryTime is null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
                return AuthResult.Fail(AuthError.RefreshTokenExpired);

            var newAccessToken = _tokenGenerator.GenerateAccessToken(user);
            var newRefreshToken = _tokenGenerator.GenerateRefreshToken();

            await _userRepository.UpdateRefreshTokenAsync(
                user.Id, newRefreshToken, DateTime.UtcNow.Add(RefreshTokenLifetime));

            return AuthResult.Success(user, newAccessToken, newRefreshToken);
        }

        public async Task<AuthResult> ChangePasswordAsync(Guid userId, string oldPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user is null)
                return AuthResult.Fail(AuthError.UserNotFound);

            var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, oldPassword);
            if (verification == PasswordVerificationResult.Failed)
                return AuthResult.Fail(AuthError.InvalidCredentials);

            if (oldPassword == newPassword)
                return AuthResult.Fail(AuthError.SamePassword);

            var newHash = _passwordHasher.HashPassword(user, newPassword);
            await _userRepository.UpdatePasswordHashAsync(user.Id, newHash);

            // Инвалидируем все сессии — при смене пароля это обязательно
            await _userRepository.UpdateRefreshTokenAsync(user.Id, null, null);

            return AuthResult.Success(user);
        }

        public async Task LogoutAsync(Guid userId)
        {
            await _userRepository.UpdateRefreshTokenAsync(userId, null, null);
        }
    }
}
