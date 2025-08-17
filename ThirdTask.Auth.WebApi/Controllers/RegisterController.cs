using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ThirdTask.Auth.Application.Dtos;
using ThirdTask.Auth.Application.Features.Mediator.Commands;
using ThirdTask.Auth.Application.Interfaces;

namespace ThirdTask.Auth.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IAuthService _authService;

        public RegisterController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserRequestDto registerUserRequestDto)
        {
            await _authService.RegisterAsync(registerUserRequestDto.Username, registerUserRequestDto.Password,
                registerUserRequestDto.Email, registerUserRequestDto.Name, registerUserRequestDto.Surname);
            return Ok("User added successfully");
        }
    }
}
