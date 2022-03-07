using Manager.Api.Binders;
using Manager.Application.Dtos;
using Manager.Application.Interfaces;
using Manager.Application.Interfaces.ThirdPartyContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.Api.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly ITokenService _tokenService;

        public AccountsController(IAccountService accountService, ITokenService tokenService)
        {
            this._accountService = accountService;
            this._tokenService = tokenService;
        }

        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (ModelState.IsValid)
            {
                var tokenDto = await _accountService.LoginAsync(loginDto);
                return Ok(tokenDto);
            }
            else
            {
                return BadRequest("User data is invalid");
            }
        }

        [HttpPost]
        [Route("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (ModelState.IsValid)
            {
                await this._accountService.RegisterAsync(registerDto);
            }

            return Ok();
        }

        [HttpPost]
        [Route("revoke-token")]
        public async Task<IActionResult> RevokeAccessToken([ModelBinder(typeof(RevokeTokenModelBinder))] RevokeTokenDto revokeTokenDto)
        {
            if (ModelState.IsValid)
            {
                var tokenDto = await _accountService.RevokeTokenAsync(revokeTokenDto);
                return Ok(tokenDto);
            }
            else
            {
                return BadRequest("Data is invalid");
            }
        }

        [Authorize]
        [HttpGet]
        [Route("current-user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CurrentUser()
        {
            if (HttpContext.Request.Headers.TryGetValue("Authorization", out var token))
            {
                var currentUser = await _accountService.GetAuthenticatedUserAsync(token);
                return Ok(currentUser);
            }

            return NotFound("Token does not exist.");
        }

        [HttpPost]
        [Route("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Logout()
        {
            if (HttpContext.Request.Headers.TryGetValue("Authorization", out var token) && _tokenService.InvalidateOrCheckAccessToken(token))
            {
                return Ok();
            }

            return NotFound("Token does not exist.");
        }

        [Authorize]
        [HttpPost("change-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            if (HttpContext.Request.Headers.TryGetValue("Authorization", out var token))
            {
                await _accountService.ChangePasswordAsync(token, changePasswordDto);

                return Ok();
            }

            return NotFound("Token does not exist.");
        }
    }
}
