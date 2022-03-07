using Manager.Application.Dtos;
using Manager.Application.Exceptions;
using Manager.Application.Interfaces.ThirdPartyContracts;
using Manager.Infrastructure.Identity.Constants;
using Manager.Infrastructure.Identity.Models;
using Manager.Infrastructure.Identity.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Manager.Infrastructure.Identity.Services
{
    public class AuthService : IAuthService
    {
        //private readonly IStringLocalizer<AuthService> _stringLocalizer;
        private readonly UserManager<IdentityAppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly JwtOption _jwtOption;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthService(UserManager<IdentityAppUser> userManager, ITokenService tokenService, IOptions<JwtOption> jwtOption, RoleManager<IdentityRole> roleManager)
        {
            //this._stringLocalizer = stringLocalizer;
            this._userManager = userManager;
            this._tokenService = tokenService;
            this._jwtOption = jwtOption.Value;
            this._roleManager = roleManager;
        }

        public async Task<string> CreateIdentityAsync(RegisterDto registerDto)
        {
            var registeredUser = await this._userManager.FindByNameAsync(registerDto.Email);
            if (registeredUser != null)
                throw new ApiException("Korisnik vec postoji.", 400);

            var user = new IdentityAppUser { UserName = registerDto.Email, Email = registerDto.Email };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            IdentityRoles.Roles.TryGetValue("User", out string role);
 
            var roleResult = await _userManager.AddToRoleAsync(user, role);


            if (!result.Succeeded)
            {
                var message = result.Errors
                    .Select(e => e.Description)
                    .Aggregate((current, next) => $"{current} {next}");

                throw new Exception(message);
            }

            if (!roleResult.Succeeded)
            {
                var message = result.Errors
                    .Select(e => e.Description)
                    .Aggregate((current, next) => $"{current} {next}");

                throw new Exception(message);
            }

            return user.Id;

        }

        public async Task<TokenDto> LoginAsync(LoginDto loginDto)
        {
            var identityUser = _userManager
                .Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefault(u => u.Email == loginDto.Email);

            if (identityUser is null)
            {
                //throw new ApiException(string.Format(_stringLocalizer["UserWithEmailNotFound"], loginDto.Email), 404);
                throw new ApiException("Korisnik čiji je Email: " + loginDto.Email + " nije pronađen.", 404);
            }

            if (!await _userManager.CheckPasswordAsync(identityUser, loginDto.Password))
            {
                //throw new ApiException(_stringLocalizer["CredentialsNotValid"], 400);
                throw new ApiException("Šifra nije ispravna.", 400);
            }

            var roles = await _userManager.GetRolesAsync(identityUser);
            var refreshToken = _tokenService.GenerateRefreshToken();
            var accessToken = _tokenService.GenerateAccessToken(identityUser.UserName, identityUser.Id, roles);
            if (identityUser.RefreshTokens is null)
            {
                identityUser.RefreshTokens = new List<RefreshToken>();
            }

            identityUser.RefreshTokens.Add(new RefreshToken { Token = refreshToken, Expires = _jwtOption.RefreshTokenExpiration });//Ovde bi brisao sve prethodne refreshToken-e jer na skavi login dodoajemo novi refreshToken
            await _userManager.UpdateAsync(identityUser);

            return new TokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Expiration = _jwtOption.Expiration
            };
        }

        public async Task<TokenDto> RevokeAsync(RevokeTokenDto revokeTokenDto)
        {
            var userNameClaim = _tokenService.GetUserNameClaimFromToken(revokeTokenDto.AccessToken);

            if (userNameClaim is null)
            {
                throw new ApiException("Token nije ispravan.", 401);
            }

            var identityUser = await _userManager
                .Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.UserName == userNameClaim.Value);

            if (identityUser is null)
            {
                throw new ApiException("Korisnik Čiji je Email: " + userNameClaim.Value + "nije pronađen.", 404);
            }

            var refreshToken = identityUser.RefreshTokens.FirstOrDefault(t => t.Token == revokeTokenDto.RefreshToken && t.Active);
            if (refreshToken is null)
            {
                throw new ApiException("Token za refrešovanje nevalidan.", 401);
            }

            var roles = await _userManager.GetRolesAsync(identityUser);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var newAccessToken = _tokenService.GenerateAccessToken(identityUser.UserName, identityUser.Id, roles);

            identityUser.RefreshTokens.Remove(refreshToken);
            identityUser.RefreshTokens.Add(new RefreshToken { Token = newRefreshToken, Expires = _jwtOption.RefreshTokenExpiration });
            await _userManager.UpdateAsync(identityUser);

            return new TokenDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                Expiration = _jwtOption.Expiration
            };

        }

        public async Task<string> FindEmailByIdAsync(string identityId)
        {
            var identityUser = await _userManager.FindByIdAsync(identityId);

            if (identityUser is null)
            {
                throw new ApiException("Korisnik čiji je Id:" + identityId + "nije pronađen.", 404);
            }

            return identityUser.Email;
        }

        public async Task ChangePasswordAsync(string identityId, ChangePasswordDto changePasswordDto)
        {
            var identityUser = await _userManager.FindByIdAsync(identityId);

            if (identityUser is null)
            {
                throw new ApiException("Korisnik čiji je Id: " + identityId + "nije pronađen.", 404);
            }

            var result = await _userManager.ChangePasswordAsync(identityUser, changePasswordDto.CurrentPassword, changePasswordDto.Password);

            if (!result.Succeeded)
            {
                var message = result.Errors
                    .Select(e => e.Description)
                    .Aggregate((current, next) => $"{current}{next}");

                throw new ApiException(message, 500);
            }
        }
        
        public async Task SetPasswordAsync(string identityId, string password)
        {
            var identityUser = await _userManager.FindByIdAsync(identityId);

            if (identityUser is null)
            {
                throw new ApiException("Korisnik čiji je Id: " + identityId + "nije pronađen.", 404);
            }

            await _userManager.RemovePasswordAsync(identityUser);
            var result = await _userManager.AddPasswordAsync(identityUser, password);

            if (!result.Succeeded)
            {
                var message = result.Errors
                    .Select(e => e.Description)
                    .Aggregate((current, next) => $"{current}{next}");

                throw new ApiException(message, 500);
            }
        }

        public async Task ChangeEmailAsync(string identityId, ChangeEmailDto changeEmailDto)
        {
            var identityUser = await _userManager.FindByIdAsync(identityId);

            if (identityUser is null)
            {
                throw new ApiException("Korisnik ciji je Id: " + identityId + "nije pronadjen.", 404);
            }

            var result = await _userManager.SetEmailAsync(identityUser, changeEmailDto.Email);

            if (!result.Succeeded)
            {
                var message = result.Errors
                    .Select(e => e.Description)
                    .Aggregate((current, next) => $"{current}{next}");

                throw new ApiException(message, 500);
            }

            // TODO: this should be a transaction
            result = await _userManager.SetUserNameAsync(identityUser, changeEmailDto.Email);

            if (!result.Succeeded)
            {
                var message = result.Errors
                    .Select(e => e.Description)
                    .Aggregate((current, next) => $"{current}{next}");

                throw new ApiException(message, 500);
            }
        }

        public async Task<string> GetAdminId()
        {
            IdentityRoles.Roles.TryGetValue("Admin", out var role);
            var admins = await _userManager.GetUsersInRoleAsync(role);
            return admins.First().Id;
        }
    }
}

