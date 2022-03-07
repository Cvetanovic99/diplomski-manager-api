using AutoMapper;
using Manager.Application.Dtos;
using Manager.Application.Interfaces;
using Manager.Application.Interfaces.ThirdPartyContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public AccountService(IAuthService authService, ITokenService tokenService, IUserService userService, IMapper mapper)
        {
            this._authService = authService;
            this._tokenService = tokenService;
            this._userService = userService;
            this._mapper = mapper;
        }
        public async Task RegisterAsync(RegisterDto registerDto)
        {
            var identityId = await this._authService.CreateIdentityAsync(registerDto);
            await _userService.CreateUserAsync(new CreateUserDto { Name = registerDto.Name, Surname = registerDto.Surname, IdentityId = identityId, Tools = registerDto.Tools });

        }

        public async Task<TokenDto> LoginAsync(LoginDto loginDto)
        {
            return await _authService.LoginAsync(loginDto);
        }

        public async Task<TokenDto> RevokeTokenAsync(RevokeTokenDto revokeTokenDto)
        {
            return await _authService.RevokeAsync(revokeTokenDto);
        }

        public async Task<UserWithEmailDto> GetAuthenticatedUserAsync(string token)
        {

            var (_, identityId) = _tokenService.GetUserClaimsFromToken(token);

            var email = await _authService.FindEmailByIdAsync(identityId);
            var user = await _userService.GetUserByIdentityId(identityId);

            var userWithEmail = _mapper.Map<UserWithEmailDto>(user);
            userWithEmail.Email = email;

            return userWithEmail;
        }

        public async Task<int> GetIdForAuthenticatedUserAsync(string token)
        {
            var(_, IdentityId) = _tokenService.GetUserClaimsFromToken(token);

            var user = await _userService.GetUserByIdentityId(IdentityId);

            return user.Id;
        }

        public async Task ChangePasswordAsync(string token, ChangePasswordDto changePasswordDto)
        {
            var (_, IdentityId) = _tokenService.GetUserClaimsFromToken(token);

            await _authService.ChangePasswordAsync(IdentityId, changePasswordDto);
        }
    }
}
