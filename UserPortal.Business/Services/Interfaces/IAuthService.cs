// Ruta: ./UserPortal.Business/Services/Interfaces/IAuthService.cs
using System.Threading.Tasks;
using UserPortal.Shared.DTOs.Request;
using UserPortal.Shared.DTOs.Response;

namespace UserPortal.Business.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDTO> LoginAsync(LoginUserDTO loginDto);
    Task<AuthResponseDTO> RegisterAsync(RegisterUserDTO registerDto);
}