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
using UserPortal.Shared.Exceptions; 

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
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Obtiene el perfil de un usuario por su ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<UserResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetUserProfile(int id)
    {
        try
        {
            var currentUserId = User.GetUserId();
            var userRole = User.GetUserRole();

            if (currentUserId != id && userRole != UserRoles.Admin)
            {
                _logger.LogWarning("Acceso denegado: Usuario {CurrentUserId} intentó acceder al perfil {UserId}", currentUserId, id);
                return StatusCode(StatusCodes.Status403Forbidden, 
                    ApiResponse<object>.CreateError("No tiene permisos para ver este perfil"));
            }

            var user = await _userService.GetUserByIdAsync(id);
            return Ok(ApiResponse<UserResponseDTO>.CreateSuccess(user, "Perfil obtenido exitosamente"));
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Usuario no encontrado: {UserId}", id);
            return NotFound(ApiResponse<object>.CreateError($"No se encontró el usuario con ID: {id}"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo perfil de usuario: {UserId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.CreateError("Error interno del servidor al obtener el perfil"));
        }
    }

    /// <summary>
    /// Obtiene una lista paginada de usuarios (solo para administradores)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<UserResponseDTO>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetUsers([FromQuery] PaginationParams parameters)
    {
        try
        {
            parameters ??= new PaginationParams();
            
            if (!parameters.IsValid())
            {
                return BadRequest(ApiResponse<object>.CreateError("Parámetros de paginación inválidos"));
            }

            var users = await _userService.GetUsersAsync(parameters);
            return Ok(ApiResponse<PaginatedResult<UserResponseDTO>>.CreateSuccess(
                users,
                $"Se encontraron {users.TotalItems} usuarios"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obteniendo lista de usuarios");
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.CreateError("Error interno del servidor al obtener la lista de usuarios"));
        }
    }

    /// <summary>
    /// Actualiza el perfil de un usuario
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<UserResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateProfile(int id, [FromBody] UpdateUserDTO updateDto)
    {
        try
        {
            if (updateDto == null)
            {
                return BadRequest(ApiResponse<object>.CreateError("No se proporcionaron datos para actualizar"));
            }

            var currentUserId = User.GetUserId();
            var userRole = User.GetUserRole();

            if (currentUserId != id && userRole != UserRoles.Admin)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiResponse<object>.CreateError("No tiene permisos para actualizar este perfil"));
            }

            var updatedUser = await _userService.UpdateUserAsync(id, updateDto);
            return Ok(ApiResponse<UserResponseDTO>.CreateSuccess(updatedUser, SuccessMessages.ProfileUpdated));
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Error de validación actualizando usuario: {UserId}", id);
            return BadRequest(ApiResponse<object>.CreateError(ex.Message, ex.Errors));
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Usuario no encontrado: {UserId}", id);
            return NotFound(ApiResponse<object>.CreateError(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error actualizando perfil de usuario: {UserId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.CreateError("Error interno del servidor al actualizar el perfil"));
        }
    }

    /// <summary>
    /// Elimina un usuario (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            await _userService.DeleteUserAsync(id);
            _logger.LogInformation("Usuario eliminado exitosamente: {UserId}", id);
            return Ok(ApiResponse<object>.CreateSuccess(message: SuccessMessages.UserDeleted));
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Usuario no encontrado para eliminar: {UserId}", id);
            return NotFound(ApiResponse<object>.CreateError(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error eliminando usuario: {UserId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.CreateError("Error interno del servidor al eliminar el usuario"));
        }
    }

    /// <summary>
    /// Busca usuarios por término de búsqueda (solo para administradores)
    /// </summary>
    [HttpGet("search")]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResult<UserResponseDTO>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchUsers(
        [FromQuery] string searchTerm,
        [FromQuery] PaginationParams parameters)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest(ApiResponse<object>.CreateError("El término de búsqueda no puede estar vacío"));
            }

            parameters ??= new PaginationParams();
            
            if (!parameters.IsValid())
            {
                return BadRequest(ApiResponse<object>.CreateError("Parámetros de paginación inválidos"));
            }

            var users = await _userService.SearchUsersAsync(searchTerm, parameters);
            return Ok(ApiResponse<PaginatedResult<UserResponseDTO>>.CreateSuccess(
                users,
                $"Se encontraron {users.TotalItems} usuarios"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error buscando usuarios con término: {SearchTerm}", searchTerm);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.CreateError("Error interno del servidor al buscar usuarios"));
        }
    }

    /// <summary>
    /// Actualiza la foto de perfil de un usuario
    /// </summary>
    [HttpPut("{id}/profile-picture")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateProfilePicture(int id, [FromForm] IFormFile file)
    {
        try
        {
            var currentUserId = User.GetUserId();
            var userRole = User.GetUserRole();

            if (currentUserId != id && userRole != UserRoles.Admin)
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    ApiResponse<object>.CreateError("No tiene permisos para actualizar esta foto de perfil"));
            }

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
                return BadRequest(ApiResponse<object>.CreateError("El archivo no debe exceder 5MB"));
            }

            var imageUrl = await _userService.UpdateProfilePictureAsync(id, file);
            return Ok(ApiResponse<string>.CreateSuccess(imageUrl, SuccessMessages.ProfileImageUpdated));
        }
        catch (ValidationException ex)
        {
            return BadRequest(ApiResponse<object>.CreateError(ex.Message));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse<object>.CreateError(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error actualizando foto de perfil: {UserId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<object>.CreateError("Error interno del servidor al actualizar la foto de perfil"));
        }
    }
}

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
