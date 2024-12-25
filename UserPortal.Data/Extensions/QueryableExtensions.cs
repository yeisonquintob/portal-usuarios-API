using System.Linq;
using Microsoft.EntityFrameworkCore;
using UserPortal.Data.Entities.Base;
using UserPortal.Shared.Models;

namespace UserPortal.Data.Extensions;

/// <summary>
/// Extensiones para consultas IQueryable
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Aplica filtro de registros activos
    /// </summary>
    public static IQueryable<T> ActiveOnly<T>(this IQueryable<T> query) where T : BaseEntity
        => query.Where(x => x.IsActive);

    /// <summary>
    /// Aplica paginación a una consulta
    /// </summary>
    public static async Task<PaginatedResult<T>> ToPaginatedResultAsync<T>(
        this IQueryable<T> query,
        PaginationParams parameters)
    {
        var totalItems = await query.CountAsync();
        var items = await query
            .Skip(parameters.Skip)
            .Take(parameters.PageSize)
            .ToListAsync();

        return PaginatedResult<T>.Create(items, totalItems, parameters);
    }
}