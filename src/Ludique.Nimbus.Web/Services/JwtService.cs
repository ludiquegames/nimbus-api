using Ludique.Nimbus.Infrastructure.Entities;
using Ludique.Nimbus.Web.Models.Identity;
using Ludique.Nimbus.Web.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ludique.Nimbus.Web.Services
{
    public class JwtService : ITokenService
    {
        private readonly SignInManager<User> _signInManager;
        private readonly SecurityKey _key;
        private readonly JwtSecurityTokenHandler _tokenHandler = new();
        private readonly JwtSettings _jwtSettings;

        public JwtService(SignInManager<User> signInManager, JwtSettings jwtSettings)
        {
            _signInManager = signInManager;
            _jwtSettings = jwtSettings;
            _key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.Secret));
        }

        public async Task<TokenModel> GenerateAsync(User user)
        {
            ArgumentNullException.ThrowIfNull(user);
            ClaimsPrincipal principal = await _signInManager.CreateUserPrincipalAsync(user);
            ClaimsIdentity identity = ClaimsPrincipal.PrimaryIdentitySelector(principal.Identities)
                ??throw new InvalidOperationException("Could not select the PrimaryClaim's identity");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddSeconds(_jwtSettings.Lifetime),
                Subject = identity,
                SigningCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256),
                Audience = _jwtSettings.Audience,
                Issuer = _jwtSettings.Issuer,
            };
            SecurityToken securityToken = _tokenHandler.CreateToken(tokenDescriptor);
            return new TokenModel
            {
                AccessToken = _tokenHandler.WriteToken(securityToken),
                ExpiresIn = _jwtSettings.Lifetime,
                TokenType = _jwtSettings.Type
            };
        }

    }
}
