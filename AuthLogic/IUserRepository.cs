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
        public Task<AppUser> GetByEmailAsync(string email);

        public Task<int> CreateAsync(AppUser user);
    }
}
