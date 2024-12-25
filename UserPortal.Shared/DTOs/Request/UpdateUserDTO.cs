namespace UserPortal.Shared.DTOs.Request;

/// <summary>
/// DTO para actualizar información del usuario
/// Todas las propiedades son opcionales
/// </summary>
public class UpdateUserDTO
{
    /// <summary>
    /// Nuevo correo electrónico
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Nuevo nombre
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Nuevo apellido
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Contraseña actual (requerida para cambios sensibles)
    /// </summary>
    public string? CurrentPassword { get; set; }

    /// <summary>
    /// Nueva contraseña
    /// </summary>
    public string? NewPassword { get; set; }

    /// <summary>
    /// Confirmación de la nueva contraseña
    /// </summary>
    public string? ConfirmNewPassword { get; set; }

    /// <summary>
    /// URL o Base64 de la imagen de perfil
    /// </summary>
    public string? ProfilePicture { get; set; }
}