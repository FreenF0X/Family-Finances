using AuthLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthLogic
{
    public interface IAuthServiceLogic
    {
        Task<AuthResult> RegisterAsync(string login, string password, string? email);
        Task<AuthResult> LoginAsync(string login, string password);
        Task<AuthResult> RefreshTokenAsync(string refreshToken);
        Task<AuthResult> ChangePasswordAsync(Guid userId, string oldPassword, string newPassword);
        Task LogoutAsync(Guid userId);
    }
}
