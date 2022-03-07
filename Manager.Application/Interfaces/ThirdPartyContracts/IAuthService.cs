using Manager.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Application.Interfaces.ThirdPartyContracts
{
    public interface IAuthService
    {
        Task<string> CreateIdentityAsync(RegisterDto registerDto);
        Task<TokenDto> LoginAsync(LoginDto loginDto);
        Task<TokenDto> RevokeAsync(RevokeTokenDto revokeTokenDto);
        Task<string> FindEmailByIdAsync(string identityId);
        Task ChangePasswordAsync(string identityId, ChangePasswordDto changePasswordDto);
        Task SetPasswordAsync(string identityId, string password);
        Task ChangeEmailAsync(string identityId, ChangeEmailDto changeEmailDto);
        Task<string> GetAdminId();
    }
}
