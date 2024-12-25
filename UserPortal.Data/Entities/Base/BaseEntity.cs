using System;

namespace UserPortal.Data.Entities.Base;

/// <summary>
/// Clase base abstracta para todas las entidades
/// Proporciona propiedades comunes de auditoría
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Identificador único de la entidad
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Fecha de creación del registro
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Usuario que creó el registro
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Fecha de última modificación
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Usuario que realizó la última modificación
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Indica si el registro está activo o ha sido eliminado lógicamente
    /// </summary>
    public bool IsActive { get; set; } = true;
}