using System.Threading.Tasks;
using UserPortal.Data.Entities;
using UserPortal.Shared.Models;

namespace UserPortal.Data.Repositories.Interfaces;

/// <summary>
/// Interfaz para el repositorio de roles
/// </summary>
public interface IRoleRepository : IGenericRepository<Role>
{
    /// <summary>
    /// Obtiene un rol por su nombre
    /// </summary>
    /// <param name="name">Nombre del rol</param>
    Task<Role?> GetByNameAsync(string name);

    /// <summary>
    /// Verifica si el rol es uno de los predefinidos del sistema
    /// </summary>
    /// <param name="roleId">ID del rol</param>
    Task<bool> IsDefaultRoleAsync(int roleId);

    /// <summary>
    /// Obtiene el rol por defecto para nuevos usuarios
    /// </summary>
    Task<Role?> GetDefaultRoleAsync();

    /// <summary>
    /// Verifica si un rol tiene usuarios asignados
    /// </summary>
    /// <param name="roleId">ID del rol</param>
    Task<bool> HasUsersAsync(int roleId);
}
