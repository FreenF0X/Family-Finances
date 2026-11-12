using AuthLogic.Models;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthLogic
{
    public interface IUserRepository
    {
        Task<IdentityUser?> GetByIdAsync(Guid id);
        Task<IdentityUser?> GetByLoginAsync(string login);
        Task<IdentityUser?> GetByRefreshTokenAsync(string refreshToken);
        Task CreateAsync(IdentityUser user);
        Task UpdateRefreshTokenAsync(Guid id, string? refreshToken, DateTime? expiry);
        Task UpdatePasswordHashAsync(Guid id, string passwordHash);
    }
}
