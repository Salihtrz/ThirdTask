using AutoMapper;
using Microsoft.AspNetCore.Identity;
using ThirdTask.Auth.Application.Features.Mediator.Results;
using ThirdTask.Auth.Application.Interfaces;
using ThirdTask.Auth.Domain.Entities;
using ThirdTask.Jwt.Dtos;
using ThirdTask.Jwt.Interfaces;

namespace ThirdTask.Auth.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IJwtTokenGenerator _jwtGenerator;
        private readonly IMapper _mapper;
        private readonly ILogService _logService;

        public AuthService(UserManager<AppUser> userManager, IJwtTokenGenerator jwtGenerator, IMapper mapper, ILogService logService)
        {
            _userManager = userManager;
            _jwtGenerator = jwtGenerator;
            _mapper = mapper;
            _logService = logService;
        }

        public async Task<TokenResponseDto> LoginAsync(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                await _logService.LogAsync("AuthService", "WARNING", $"Login failed: User '{username}' not found.");
                throw new Exception("User not found!");
            }

            var valid = await _userManager.CheckPasswordAsync(user, password);
            if (!valid)
            {
                await _logService.LogAsync("AuthService", "WARNING", $"Login failed: Incorrect password for user '{username}'.");
                throw new Exception("Password incorrect!");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var result = new GetCheckAppUserQueryResult
            {
                Id = user.Id,
                IsExist = true,
                Username = user.UserName,
                Role = roles.FirstOrDefault()
            };

            var value = new CheckAppUserDto
            {
                Id = result.Id,
                Username = result.Username,
                IsExist = result.IsExist,
                Role = result.Role
            };

            var accessToken = _jwtGenerator.GenerateToken(value);

            var refreshToken = Guid.NewGuid().ToString();
            var refreshTokenExpireDate = DateTime.UtcNow.AddDays(3);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpireDate = refreshTokenExpireDate;
            await _userManager.UpdateAsync(user);

            await _logService.LogAsync("AuthService", "INFO", $"User '{username}' logged in successfully.");
            return new TokenResponseDto(accessToken.Token, accessToken.ExpireDate, refreshToken, refreshTokenExpireDate);
        }

        public async Task<TokenResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.RefreshToken == refreshToken);
            if (user == null || user.RefreshTokenExpireDate < DateTime.UtcNow)
            {
                await _logService.LogAsync("AuthService", "WARNING", "Refresh token is invalid or expired.");
                throw new Exception("Refresh Token is invalid or expired!");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var value = new CheckAppUserDto
            {
                Id = user.Id,
                Username = user.UserName,
                IsExist = true,
                Role = roles.FirstOrDefault()
            };

            var accessToken = _jwtGenerator.GenerateToken(value);

            var newRefreshToken = Guid.NewGuid().ToString();
            var newRefreshTokenExpireDate = DateTime.UtcNow.AddDays(3);

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpireDate = newRefreshTokenExpireDate;
            await _userManager.UpdateAsync(user);

            await _logService.LogAsync("AuthService", "INFO", $"Refresh token generated for user '{user.UserName}'.");
            return new TokenResponseDto(accessToken.Token, accessToken.ExpireDate, newRefreshToken, newRefreshTokenExpireDate);
        }

        public async Task RegisterAsync(string username, string password, string email, string name, string surname)
        {
            var user = new AppUser
            {
                UserName = username,
                Name = name,
                Email = email,
                Surname = surname,
                RefreshToken = "",
                RefreshTokenExpireDate = DateTime.UtcNow,
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(err => err.Description));
                await _logService.LogAsync("AuthService", "ERROR", $"User registration failed for '{username}': {errors}");
                throw new Exception(errors);
            }

            await _userManager.AddToRoleAsync(user, "Writer");
            await _logService.LogAsync("AuthService", "INFO", $"User '{username}' registered successfully with role 'Writer'.");
        }
    }
}
