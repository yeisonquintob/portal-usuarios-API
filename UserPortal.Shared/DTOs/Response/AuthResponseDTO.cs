// AuthResponseDTO.cs
namespace UserPortal.Shared.DTOs.Response;

/// <summary>
/// DTO para respuesta de autenticación exitosa
/// </summary>
public class AuthResponseDTO
{
    /// <summary>
    /// Token JWT para autenticación
    /// </summary>
    public required string AccessToken { get; set; }

    /// <summary>
    /// Token de actualización para renovar el AccessToken
    /// </summary>
    public required string RefreshToken { get; set; }

    /// <summary>
    /// Fecha y hora de expiración del AccessToken
    /// </summary>
    public required DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Información del usuario autenticado
    /// </summary>
    public required UserResponseDTO User { get; set; }

    /// <summary>
    /// Tipo de token (siempre "Bearer")
    /// </summary>
    public string TokenType { get; set; } = "Bearer";
}