using Manager.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Application.Interfaces
{
    public interface IAccountService
    {
        Task RegisterAsync(RegisterDto registerDto);
        Task<TokenDto> LoginAsync(LoginDto loginDto);
        Task<TokenDto> RevokeTokenAsync(RevokeTokenDto revokeTokenDto);
        Task<UserWithEmailDto> GetAuthenticatedUserAsync(string token);
        Task<int> GetIdForAuthenticatedUserAsync(string token);
        Task ChangePasswordAsync(string token, ChangePasswordDto changePasswordDto);
    }
}
