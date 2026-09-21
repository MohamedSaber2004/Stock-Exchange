using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Stock_Exchange.Application.Common.Interfaces;
using Stock_Exchange.Application.Common.Options;
using Stock_Exchange.Domain.Entities;
using Stock_Exchange.Domain.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Stock_Exchange.Infrastructure.Services
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtSettings _settings;

        public JwtTokenService(IOptions<JwtSettings> settings)
        {
            _settings = settings.Value;
        }

        public string GenerateAccessToken(ApplicationUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("FullName", user.FullName)
            };

            var userTypesMask = roles
                .Select(r => Enum.TryParse<UserType>(r, ignoreCase: true, out var ut) ? (int)ut : 0)
                .Aggregate(0, (acc, val) => acc | val);

            claims.Add(new Claim("UserTypes", userTypesMask.ToString()));

            foreach (var role in roles)
            {
                claims.Add(new Claim("role", role));
            }

            var secret = !string.IsNullOrWhiteSpace(_settings.Secret)
                ? _settings.Secret
                : "n]:#J:?,{%9SvotDc^+/FMs7XHl$R1D2c^,Sf7_6vGJ>L8^!WvK1$$BqjVjD}rHGp}[fxYa90K1%4l3yf;sx5:";
            var issuer = !string.IsNullOrWhiteSpace(_settings.Issuer) ? _settings.Issuer : "StockExchangeAPI";
            var audience = !string.IsNullOrWhiteSpace(_settings.Audience) ? _settings.Audience : "StockExchangeMobile,StockExchangeDashboard";
            var expiryDays = _settings.ExpiryInDays > 0 ? _settings.ExpiryInDays : 30;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.Now.AddDays(expiryDays);

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: expiry,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("TokenType", "RefreshToken")
            };

            var secret = !string.IsNullOrWhiteSpace(_settings.Secret)
                ? _settings.Secret
                : "n]:#J:?,{%9SvotDc^+/FMs7XHl$R1D2c^,Sf7_6vGJ>L8^!WvK1$$BqjVjD}rHGp}[fxYa90K1%4l3yf;sx5:";
            var issuer = !string.IsNullOrWhiteSpace(_settings.Issuer) ? _settings.Issuer : "StockExchangeAPI";
            var audience = !string.IsNullOrWhiteSpace(_settings.Audience) ? _settings.Audience : "StockExchangeMobile,StockExchangeDashboard";
            var refreshDays = _settings.RefreshTokenExpiryDays > 0 ? _settings.RefreshTokenExpiryDays : 30;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.Now.AddDays(refreshDays);

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: expiry,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
