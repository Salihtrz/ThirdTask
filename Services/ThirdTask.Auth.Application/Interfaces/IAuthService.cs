using ThirdTask.Jwt.Dtos;

namespace ThirdTask.Auth.Application.Interfaces
{
    public interface IAuthService
    {
        Task<TokenResponseDto> LoginAsync(string username, string password);
        Task<TokenResponseDto> RefreshTokenAsync(string token);
        Task RegisterAsync(string username, string password, string email, string name, string surname);
    }
}
