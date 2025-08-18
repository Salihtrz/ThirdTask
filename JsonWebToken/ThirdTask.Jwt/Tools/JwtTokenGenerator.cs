using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ThirdTask.Jwt.Dtos;
using ThirdTask.Jwt.Interfaces;

namespace ThirdTask.Jwt.Tools
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        public TokenResponseDto GenerateToken(CheckAppUserDto result)
        {
            var claims = new List<Claim>();
            if (!string.IsNullOrWhiteSpace(result.Role))
                claims.Add(new Claim(ClaimTypes.Role, result.Role));

            claims.Add(new Claim(ClaimTypes.NameIdentifier, result.Id.ToString()));
            if (!string.IsNullOrWhiteSpace(result.Username))
                claims.Add(new Claim("Username", result.Username));

            if (result.Permissions != null && result.Permissions.Any())
            {
                foreach (var permission in result.Permissions)
                {
                    claims.Add(new Claim("Permission", permission));
                }
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtTokenDefaults.Key));

            var signinCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expireDate = DateTime.UtcNow.AddDays(JwtTokenDefaults.Expire);

            JwtSecurityToken token = new JwtSecurityToken(issuer: JwtTokenDefaults.ValidIssuer,
                audience: JwtTokenDefaults.ValidAudience, claims: claims, notBefore: DateTime.UtcNow,
                expires: expireDate, signingCredentials: signinCredentials);

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            return new TokenResponseDto(tokenHandler.WriteToken(token), expireDate);
        }
    }
}
