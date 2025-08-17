using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThirdTask.Auth.Application.Dtos
{
    public class TokenResponseDto
    {
        public TokenResponseDto(string token, DateTime expireDate, string refreshToken, DateTime refreshTokenExpireDate)
        {
            Token = token;
            ExpireDate = expireDate;
            RefreshToken = refreshToken;
            RefreshTokenExpireDate = refreshTokenExpireDate;
        }
        public TokenResponseDto(string token, DateTime expireDate)
        {
            Token = token;
            ExpireDate = expireDate;
        }
        public string Token { get; set; }
        public DateTime ExpireDate { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpireDate { get; set; }
    }
}
