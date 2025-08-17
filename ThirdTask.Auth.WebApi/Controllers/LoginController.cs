using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ThirdTask.Auth.Application.Dtos;
using ThirdTask.Auth.Application.Features.Mediator.Queries;
using ThirdTask.Auth.Application.Interfaces;

namespace ThirdTask.Auth.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IAuthService _authService;

        public LoginController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            var token = await _authService.LoginAsync(loginRequestDto.Username, loginRequestDto.Password);
            return Ok(token);
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(string refreshToken)
        {
            var token = await _authService.RefreshTokenAsync(refreshToken);
            return Ok(token);
        }
    }
}
