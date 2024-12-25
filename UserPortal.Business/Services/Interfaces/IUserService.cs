// Ruta: ./UserPortal.Business/Services/Interfaces/IUserService.cs
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using UserPortal.Shared.DTOs.Request;
using UserPortal.Shared.DTOs.Response;
using UserPortal.Shared.Models;

namespace UserPortal.Business.Services.Interfaces;

public interface IUserService
{
    Task<UserResponseDTO> GetUserByIdAsync(int userId);
    Task<PaginatedResult<UserResponseDTO>> GetUsersAsync(PaginationParams parameters);
    Task<UserResponseDTO> UpdateUserAsync(int userId, UpdateUserDTO updateDto);
    Task DeleteUserAsync(int userId);
    Task<PaginatedResult<UserResponseDTO>> SearchUsersAsync(string searchTerm, PaginationParams parameters);
    Task<string> UpdateProfilePictureAsync(int userId, IFormFile file);
}