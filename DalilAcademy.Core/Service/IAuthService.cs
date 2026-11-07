using DalilAcademy.Core.DTOs.Auth;

namespace DalilAcademy.Core.Service;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
    Task<LoginResponseDto> RegisterAsync(RegisterRequestDto dto);
}
