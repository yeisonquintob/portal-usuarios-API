using System;
using UserPortal.Data.Entities.Base;

namespace UserPortal.Data.Entities;

/// <summary>
/// Entidad que representa un token de actualización
/// </summary>
public class RefreshToken : BaseEntity
{
    /// <summary>
    /// Token de actualización
    /// </summary>
    public required string Token { get; set; }

    /// <summary>
    /// Fecha de expiración del token
    /// </summary>
    public required DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Fecha en que fue usado por última vez
    /// </summary>
    public DateTime? UsedAt { get; set; }

    /// <summary>
    /// Fecha en que fue invalidado
    /// </summary>
    public DateTime? InvalidatedAt { get; set; }

    /// <summary>
    /// ID del usuario al que pertenece el token
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Usuario al que pertenece el token
    /// </summary>
    public virtual User? User { get; set; }

    /// <summary>
    /// Verifica si el token de actualización está vigente
    /// </summary>
    public bool IsValidToken => 
        UsedAt == null && 
        InvalidatedAt == null && 
        ExpiresAt > DateTime.UtcNow &&
        base.IsActive;
}