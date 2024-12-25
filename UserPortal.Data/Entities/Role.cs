using System;
using System.Collections.Generic;
using UserPortal.Data.Entities.Base;

namespace UserPortal.Data.Entities;

/// <summary>
/// Entidad que representa un rol en el sistema
/// </summary>
public class Role : BaseEntity
{
    /// <summary>
    /// Nombre del rol
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Descripción del rol
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Usuarios que tienen este rol
    /// </summary>
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}