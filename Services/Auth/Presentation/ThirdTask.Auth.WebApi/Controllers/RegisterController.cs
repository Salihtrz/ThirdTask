using Microsoft.AspNetCore.Mvc;
using ThirdTask.Auth.Application.Dtos;
using ThirdTask.Auth.Application.Interfaces;

namespace ThirdTask.Auth.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogService _logService;

        public RegisterController(IAuthService authService, ILogService logService)
        {
            _authService = authService;
            _logService = logService;
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserRequestDto registerUserRequestDto)
        {
            await _logService.LogAsync("RegisterController", "INFO", $"Register attempt for user '{registerUserRequestDto.Username}' started.");

            await _authService.RegisterAsync(
                registerUserRequestDto.Username,
                registerUserRequestDto.Password,
                registerUserRequestDto.Email,
                registerUserRequestDto.Name,
                registerUserRequestDto.Surname
            );

            await _logService.LogAsync("RegisterController", "INFO", $"User '{registerUserRequestDto.Username}' registered successfully.");
            return Ok("User added successfully");
        }
    }
}
