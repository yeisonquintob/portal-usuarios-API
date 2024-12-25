using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserPortal.Data.Exceptions;

namespace UserPortal.Data.Extensions;

/// <summary>
/// Extensiones para el DbContext
/// </summary>
public static class DbContextExtensions
{
    /// <summary>
    /// Guarda los cambios con validación y manejo de errores
    /// </summary>
    public static async Task<bool> SaveChangesWithValidationAsync(
        this DbContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new DataValidationException(
                "Los datos han sido modificados por otro usuario",
                new Dictionary<string, string[]>());
        }
        catch (DbUpdateException ex)
        {
            // Manejo específico de errores de SQL Server
            if (ex.InnerException?.Message.Contains("IX_") ?? false)
            {
                throw new DataValidationException(
                    "Ya existe un registro con los mismos valores únicos",
                    new Dictionary<string, string[]>());
            }

            throw new DatabaseException(
                "Error al guardar los cambios en la base de datos",
                ex);
        }
        catch (Exception ex)
        {
            throw new DatabaseException(
                "Error inesperado al guardar los cambios",
                ex);
        }
    }
}
