using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using UserPortal.Shared.DTOs.Request;
using UserPortal.Shared.DTOs.Response;
using UserPortal.Shared.Models;
using UserPortal.Shared.Exceptions;

namespace UserPortal.Business.Services.Interfaces;

/// <summary>
/// Interfaz que define las operaciones disponibles para el servicio de usuarios
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Obtiene un usuario por su identificador
    /// </summary>
    /// <param name="userId">ID del usuario a obtener</param>
    /// <returns>DTO con la información del usuario</returns>
    /// <exception cref="NotFoundException">Si el usuario no existe</exception>
    Task<UserResponseDTO> GetUserByIdAsync(int userId);

    /// <summary>
    /// Obtiene una lista paginada de usuarios
    /// </summary>
    /// <param name="parameters">Parámetros de paginación</param>
    /// <returns>Resultado paginado con los usuarios</returns>
    Task<PaginatedResult<UserResponseDTO>> GetUsersAsync(PaginationParams parameters);

    /// <summary>
    /// Actualiza la información de un usuario
    /// </summary>
    /// <param name="userId">ID del usuario a actualizar</param>
    /// <param name="updateDto">DTO con los datos a actualizar</param>
    /// <returns>DTO con la información actualizada del usuario</returns>
    /// <exception cref="NotFoundException">Si el usuario no existe</exception>
    /// <exception cref="ValidationException">Si los datos de actualización son inválidos</exception>
    Task<UserResponseDTO> UpdateUserAsync(int userId, UpdateUserDTO updateDto);

    /// <summary>
    /// Elimina un usuario del sistema (eliminación lógica)
    /// </summary>
    /// <param name="userId">ID del usuario a eliminar</param>
    /// <exception cref="NotFoundException">Si el usuario no existe</exception>
    /// <exception cref="ValidationException">Si el usuario no puede ser eliminado (ej: último administrador)</exception>
    Task DeleteUserAsync(int userId);

    /// <summary>
    /// Busca usuarios que coincidan con un término de búsqueda
    /// </summary>
    /// <param name="searchTerm">Término para filtrar usuarios</param>
    /// <param name="parameters">Parámetros de paginación</param>
    /// <returns>Resultado paginado con los usuarios que coinciden con la búsqueda</returns>
    /// <exception cref="ValidationException">Si el término de búsqueda es inválido</exception>
    Task<PaginatedResult<UserResponseDTO>> SearchUsersAsync(
        string searchTerm,
        PaginationParams parameters);

    /// <summary>
    /// Actualiza la foto de perfil de un usuario
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="file">Archivo de imagen a subir</param>
    /// <returns>URL de la nueva imagen de perfil</returns>
    /// <exception cref="NotFoundException">Si el usuario no existe</exception>
    /// <exception cref="ValidationException">Si el archivo no cumple con los requisitos</exception>
    /// <remarks>
    /// El archivo debe ser una imagen (JPEG, PNG o GIF) y no exceder 5MB.
    /// La imagen anterior será eliminada automáticamente si existe.
    /// </remarks>
    Task<string> UpdateProfilePictureAsync(int userId, IFormFile file);
}
