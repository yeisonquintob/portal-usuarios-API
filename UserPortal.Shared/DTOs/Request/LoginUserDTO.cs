namespace UserPortal.Shared.DTOs.Request;

/// <summary>
/// DTO para el inicio de sesión
/// </summary>
public class LoginUserDTO
{
    /// <summary>
    /// Nombre de usuario o email para iniciar sesión
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// Contraseña del usuario
    /// </summary>
    public required string Password { get; set; }
}