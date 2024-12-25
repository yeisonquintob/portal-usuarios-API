using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserPortal.Data.Context;
using UserPortal.Data.Entities.Base;
using UserPortal.Data.Repositories.Interfaces;
using UserPortal.Shared.Models;

namespace UserPortal.Data.Repositories.Implementations;

/// <summary>
/// Implementación genérica del repositorio base
/// </summary>
/// <typeparam name="T">Tipo de entidad que hereda de BaseEntity</typeparam>
public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    /// <inheritdoc/>
    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    /// <inheritdoc/>
    public virtual async Task<PaginatedResult<T>> GetAllPaginatedAsync(PaginationParams parameters)
    {
        var query = _dbSet.Where(x => x.IsActive);
        var totalItems = await query.CountAsync();
        
        var items = await query
            .Skip(parameters.Skip)
            .Take(parameters.PageSize)
            .ToListAsync();

        return PaginatedResult<T>.Create(items, totalItems, parameters);
    }

    /// <inheritdoc/>
    public virtual async Task<PaginatedResult<T>> FindPaginatedAsync(
        Expression<Func<T, bool>> predicate, 
        PaginationParams parameters)
    {
        var query = _dbSet.Where(predicate).Where(x => x.IsActive);
        var totalItems = await query.CountAsync();
        
        var items = await query
            .Skip(parameters.Skip)
            .Take(parameters.PageSize)
            .ToListAsync();

        return PaginatedResult<T>.Create(items, totalItems, parameters);
    }

    /// <inheritdoc/>
    public virtual async Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    /// <inheritdoc/>
    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    /// <inheritdoc/>
    public virtual async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    /// <inheritdoc/>
    public virtual async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    /// <inheritdoc/>
    public virtual void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    /// <inheritdoc/>
    public virtual void Delete(T entity)
    {
        entity.IsActive = false;
        Update(entity);
    }

    /// <inheritdoc/>
    public virtual void DeleteRange(IEnumerable<T> entities)
    {
        foreach (var entity in entities)
        {
            entity.IsActive = false;
        }
        _dbSet.UpdateRange(entities);
    }
}