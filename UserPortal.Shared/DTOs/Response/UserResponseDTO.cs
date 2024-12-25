// UserResponseDTO.cs
namespace UserPortal.Shared.DTOs.Response;

/// <summary>
/// DTO para enviar información del usuario al cliente
/// </summary>
public class UserResponseDTO
{
    /// <summary>
    /// Identificador único del usuario
    /// </summary>
    public required int UserId { get; set; }

    /// <summary>
    /// Nombre de usuario único
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// Correo electrónico del usuario
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Nombre del usuario
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// Apellido del usuario
    /// </summary>
    public required string LastName { get; set; }

    /// <summary>
    /// URL o Base64 de la imagen de perfil (opcional)
    /// </summary>
    public string? ProfilePicture { get; set; }

    /// <summary>
    /// Rol del usuario en el sistema
    /// </summary>
    public required string Role { get; set; }

    /// <summary>
    /// Fecha de creación de la cuenta
    /// </summary>
    public required DateTime CreatedAt { get; set; }

    /// <summary>
    /// Fecha del último inicio de sesión
    /// </summary>
    public DateTime? LastLogin { get; set; }
}