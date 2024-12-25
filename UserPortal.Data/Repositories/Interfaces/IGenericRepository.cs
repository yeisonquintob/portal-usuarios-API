using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UserPortal.Data.Entities.Base;
using UserPortal.Shared.Models;

namespace UserPortal.Data.Repositories.Interfaces;

/// <summary>
/// Interfaz genérica para operaciones básicas de repositorio
/// </summary>
/// <typeparam name="T">Tipo de entidad que hereda de BaseEntity</typeparam>
public interface IGenericRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Obtiene una entidad por su identificador
    /// </summary>
    /// <param name="id">Identificador de la entidad</param>
    Task<T?> GetByIdAsync(int id);

    /// <summary>
    /// Obtiene todas las entidades de forma paginada
    /// </summary>
    /// <param name="parameters">Parámetros de paginación</param>
    Task<PaginatedResult<T>> GetAllPaginatedAsync(PaginationParams parameters);

    /// <summary>
    /// Obtiene entidades basadas en un predicado de forma paginada
    /// </summary>
    /// <param name="predicate">Expresión de filtrado</param>
    /// <param name="parameters">Parámetros de paginación</param>
    Task<PaginatedResult<T>> FindPaginatedAsync(Expression<Func<T, bool>> predicate, PaginationParams parameters);

    /// <summary>
    /// Busca una única entidad basada en un predicado
    /// </summary>
    /// <param name="predicate">Expresión de filtrado</param>
    Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Verifica si existe alguna entidad que cumpla con el predicado
    /// </summary>
    /// <param name="predicate">Expresión de filtrado</param>
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Añade una nueva entidad
    /// </summary>
    /// <param name="entity">Entidad a añadir</param>
    Task AddAsync(T entity);

    /// <summary>
    /// Añade un rango de entidades
    /// </summary>
    /// <param name="entities">Entidades a añadir</param>
    Task AddRangeAsync(IEnumerable<T> entities);

    /// <summary>
    /// Actualiza una entidad existente
    /// </summary>
    /// <param name="entity">Entidad a actualizar</param>
    void Update(T entity);

    /// <summary>
    /// Elimina una entidad de forma lógica
    /// </summary>
    /// <param name="entity">Entidad a eliminar</param>
    void Delete(T entity);

    /// <summary>
    /// Elimina un rango de entidades de forma lógica
    /// </summary>
    /// <param name="entities">Entidades a eliminar</param>
    void DeleteRange(IEnumerable<T> entities);
}