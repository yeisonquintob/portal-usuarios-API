// Ruta: ./UserPortal.API/Controllers/UserController.cs
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UserPortal.Business.Services.Interfaces;
using UserPortal.Shared.Constants;
using UserPortal.Shared.DTOs.Request;
using UserPortal.Shared.DTOs.Response;
using UserPortal.Shared.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace UserPortal.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(
        IUserService userService,
        ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene el perfil de un usuario por su ID
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <returns>Información del perfil del usuario</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<UserResponseDTO>), 200)]
    [ProducesResponseType(typeof(ApiResponse<>), 404)]
    public async Task<IActionResult> GetUserProfile(int id)
    {
        try
        {
            // Verificar si el usuario actual tiene permiso para ver este perfil
            var currentUserId = User.GetUserId();
            var userRole = User.GetUserRole();

            if (currentUserId != id && userRole != UserRoles.Admin)
            {
                return Forbid();
            }

            var user = await _userService.GetUserByIdAsync(id);
            return Ok(ApiResponse<UserResponseDTO>.CreateSuccess(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo perfil de usuario: {UserId}", id);
            throw;
        }
    }

    /// <summary>
    /// Obtiene una lista paginada de usuarios (solo para administradores)
    /// </summary>
    /// <param name="parameters">Parámetros de paginación</param>
    /// <returns>Lista paginada de usuarios</returns>
    [HttpGet]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<UserResponseDTO>>), 200)]
    public async Task<IActionResult> GetUsers([FromQuery] PaginationParams parameters)
    {
        try
        {
            var users = await _userService.GetUsersAsync(parameters);
            return Ok(ApiResponse<PaginatedResult<UserResponseDTO>>.CreateSuccess(users));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo lista de usuarios");
            throw;
        }
    }

    /// <summary>
    /// Actualiza el perfil de un usuario
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="updateDto">Datos a actualizar</param>
    /// <returns>Perfil actualizado</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<UserResponseDTO>), 200)]
    [ProducesResponseType(typeof(ApiResponse<>), 400)]
    [ProducesResponseType(typeof(ApiResponse<>), 404)]
    public async Task<IActionResult> UpdateProfile(int id, [FromBody] UpdateUserDTO updateDto)
    {
        try
        {
            // Verificar si el usuario actual tiene permiso para actualizar este perfil
            var currentUserId = User.GetUserId();
            var userRole = User.GetUserRole();

            if (currentUserId != id && userRole != UserRoles.Admin)
            {
                return Forbid();
            }

            var updatedUser = await _userService.UpdateUserAsync(id, updateDto);
            return Ok(ApiResponse<UserResponseDTO>.CreateSuccess(
                updatedUser, 
                SuccessMessages.ProfileUpdated));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error actualizando perfil de usuario: {UserId}", id);
            throw;
        }
    }

    /// <summary>
    /// Elimina un usuario (soft delete)
    /// </summary>
    /// <param name="id">ID del usuario a eliminar</param>
    /// <returns>Resultado de la operación</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<>), 200)]
    [ProducesResponseType(typeof(ApiResponse<>), 404)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            await _userService.DeleteUserAsync(id);
            return Ok(ApiResponse<object>.CreateSuccess(
                message: SuccessMessages.UserDeleted));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error eliminando usuario: {UserId}", id);
            throw;
        }
    }

    /// <summary>
    /// Busca usuarios por término de búsqueda (solo para administradores)
    /// </summary>
    /// <param name="searchTerm">Término de búsqueda</param>
    /// <param name="parameters">Parámetros de paginación</param>
    /// <returns>Lista paginada de usuarios que coinciden con la búsqueda</returns>
    [HttpGet("search")]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<UserResponseDTO>>), 200)]
    public async Task<IActionResult> SearchUsers(
        [FromQuery] string searchTerm,
        [FromQuery] PaginationParams parameters)
    {
        try
        {
            var users = await _userService.SearchUsersAsync(searchTerm, parameters);
            return Ok(ApiResponse<PaginatedResult<UserResponseDTO>>.CreateSuccess(users));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error buscando usuarios con término: {SearchTerm}", searchTerm);
            throw;
        }
    }

    /// <summary>
    /// Actualiza la foto de perfil de un usuario
    /// </summary>
    /// <param name="id">ID del usuario</param>
    /// <param name="file">Archivo de imagen</param>
    /// <returns>URL de la nueva imagen de perfil</returns>
    [HttpPut("{id}/profile-picture")]
    [ProducesResponseType(typeof(ApiResponse<string>), 200)]
    [ProducesResponseType(typeof(ApiResponse<>), 400)]
    public async Task<IActionResult> UpdateProfilePicture(int id, [FromForm] IFormFile file)
    {
        try
        {
            // Verificar permisos
            var currentUserId = User.GetUserId();
            var userRole = User.GetUserRole();

            if (currentUserId != id && userRole != UserRoles.Admin)
            {
                return Forbid();
            }

            // Validar archivo
            if (file == null || file.Length == 0)
            {
                return BadRequest(ApiResponse<object>.CreateError("No se ha proporcionado ningún archivo"));
            }

            if (!file.ContentType.StartsWith("image/"))
            {
                return BadRequest(ApiResponse<object>.CreateError("El archivo debe ser una imagen"));
            }

            if (file.Length > 5 * 1024 * 1024) // 5MB
            {
                return BadRequest(ApiResponse<object>.CreateError("El archivo es demasiado grande"));
            }

            // Procesar y guardar imagen
            var imageUrl = await _userService.UpdateProfilePictureAsync(id, file);
            
            return Ok(ApiResponse<string>.CreateSuccess(
                imageUrl, 
                SuccessMessages.ProfileImageUpdated));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error actualizando foto de perfil: {UserId}", id);
            throw;
        }
    }
}

// Extensiones de ClaimsPrincipal para obtener información del usuario actual
public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    public static string GetUserRole(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
    }
}