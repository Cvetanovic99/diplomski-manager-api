using Manager.Application.Interfaces.ThirdPartyContracts;
using Manager.Infrastructure.Identity.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Infrastructure.Identity.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtOption _jwtOptions;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

        public TokenService(JwtSecurityTokenHandler jwtSecurityTokenHandler, IOptions<JwtOption> jwtOption)
        {
            this._jwtSecurityTokenHandler = jwtSecurityTokenHandler;
            this._jwtOptions = jwtOption.Value;
        }
        public string GenerateAccessToken(string userName, string userId, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.Jti, _jwtOptions.Jti),
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.NameIdentifier, userId)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: _jwtOptions.NotBefore,
                expires: _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SigninCredentials);

            return _jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var randomNumberGenerator = RandomNumberGenerator.Create();

            randomNumberGenerator.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public bool InvalidateOrCheckAccessToken(string token, bool check = false)
        {
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            var principals = GetPrincipalFromToken(token);
            var jtiClaim = principals?.Claims.SingleOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti);
            var expClaim = principals?.Claims.SingleOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp);


            if (jtiClaim is null || expClaim is null)
            {
                return false;
            }

            if (!double.TryParse(expClaim.Value, out double tokenExpirationTimestamp))
            {
                return false;
            }


            //if (check)
            //{
            //    return _cache.Get(jtiClaim.Value) is null;
            //}

            //var memoryCacheOptions = new DistributedCacheEntryOptions
            //{
            //    AbsoluteExpiration = DateTimeOffset.UtcNow + GetExpirationFromTimestamp(tokenExpirationTimestamp)
            //};

            //_cache.Set(jtiClaim.Value, Encoding.UTF8.GetBytes(token), memoryCacheOptions);

            return true;
        }

        public ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            token = token.Replace("Bearer ", string.Empty);
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtOptions.SigningKey));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _jwtOptions.Issuer,

                ValidateAudience = true,
                ValidAudience = _jwtOptions.Audience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            var claimsPrincipals =
                _jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken
                || (jwtSecurityToken.Issuer != _jwtOptions.Issuer)
                || (jwtSecurityToken.ValidTo < DateTime.UtcNow))
            {
                throw new SecurityTokenException("The token is not valid.");
            }

            return claimsPrincipals;
        }

        public (string email, string identityId) GetUserClaimsFromToken(string token)
        {
            token = token.Replace("Bearer ", string.Empty);

            var jwtToken = _jwtSecurityTokenHandler.ReadJwtToken(token);

            var identityId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            var userName = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);

            return new(userName.Value, identityId.Value);
        }

        public Claim GetUserNameClaimFromToken(string token)
        {
            token = token.Replace("Bearer ", string.Empty);//Mada nema potrebe zato sto se vec "Bearer " skida u ModelBInder-u sa accessToken-a ali opet je napisano
            var jwtToken = _jwtSecurityTokenHandler.ReadJwtToken(token);

            var userName = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);

            return userName;
        }
    }
}
