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
        Task<IdentityUser?> GetByLoginAsync(string login);
        Task CreateAsync(IdentityUser user);
        Task UpdateRefreshTokenAsync(Guid id, string? refreshToken, DateTime? expiry);
    }
}
