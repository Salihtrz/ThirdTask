using Microsoft.AspNetCore.Mvc;
using ThirdTask.Auth.Application.Dtos;
using ThirdTask.Auth.Application.Interfaces;

namespace ThirdTask.Auth.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogService _logService;

        public LoginController(IAuthService authService, ILogService logService)
        {
            _authService = authService;
            _logService = logService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            await _logService.LogAsync("LoginController", "INFO", $"Login attempt for user '{loginRequestDto.Username}' started.");

            var token = await _authService.LoginAsync(loginRequestDto.Username, loginRequestDto.Password);

            await _logService.LogAsync("LoginController", "INFO", $"Login successful for user '{loginRequestDto.Username}'.");
            return Ok(token);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenRequestDto request)
        {
            await _logService.LogAsync("LoginController", "INFO", $"Refresh token attempt started.");

            var token = await _authService.RefreshTokenAsync(request.RefreshToken);

            await _logService.LogAsync("LoginController", "INFO", $"Refresh token successful.");
            return Ok(token);
        }
    }
}
