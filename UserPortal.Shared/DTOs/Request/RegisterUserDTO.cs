namespace UserPortal.Shared.DTOs.Request;

/// <summary>
/// DTO para el registro de nuevos usuarios
/// </summary>
public class RegisterUserDTO
{
    /// <summary>
    /// Nombre de usuario único en el sistema
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// Correo electrónico único del usuario
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Contraseña del usuario
    /// </summary>
    public required string Password { get; set; }

    /// <summary>
    /// Confirmación de la contraseña
    /// </summary>
    public required string ConfirmPassword { get; set; }

    /// <summary>
    /// Nombre del usuario
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// Apellido del usuario
    /// </summary>
    public required string LastName { get; set; }
}