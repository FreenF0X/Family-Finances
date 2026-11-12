using AuthLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthLogic
{
    public interface ITokenGenerator
    {
        string GenerateAccessToken(IdentityUser user);
        string GenerateRefreshToken();
    }
}
