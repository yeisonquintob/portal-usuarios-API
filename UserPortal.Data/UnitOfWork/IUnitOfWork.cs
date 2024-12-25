using System;
using System.Threading.Tasks;
using UserPortal.Data.Repositories.Interfaces;

namespace UserPortal.Data.UnitOfWork;

/// <summary>
/// Interfaz para el patrón Unit of Work
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Repositorio de usuarios
    /// </summary>
    IUserRepository Users { get; }

    /// <summary>
    /// Repositorio de roles
    /// </summary>
    IRoleRepository Roles { get; }

    /// <summary>
    /// Guarda todos los cambios realizados en el contexto
    /// </summary>
    Task<int> SaveChangesAsync();
}
