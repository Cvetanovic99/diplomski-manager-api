using Manager.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Application.Interfaces.ThirdPartyContracts
{
    public interface ITokenService
    {
        string GenerateAccessToken(string userName, string userId, IList<string> roles);
        string GenerateRefreshToken();
        bool InvalidateOrCheckAccessToken(string token, bool check = false);
        ClaimsPrincipal GetPrincipalFromToken(string token);

        (string email, string identityId) GetUserClaimsFromToken(string token);
        Claim GetUserNameClaimFromToken(string token);
    }
}
