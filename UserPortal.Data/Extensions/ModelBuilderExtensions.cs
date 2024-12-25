using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using UserPortal.Data.Entities.Base;

namespace UserPortal.Data.Extensions;

/// <summary>
/// Extensiones para la configuración global del ModelBuilder
/// </summary>
public static class ModelBuilderExtensions
{
    /// <summary>
    /// Aplica la configuración de auditoría a todas las entidades
    /// </summary>
    public static void ApplyAuditableConfiguration(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                // Configurar CreatedAt como no nulo y con valor por defecto
                modelBuilder.Entity(entityType.Name).Property("CreatedAt")
                    .IsRequired()
                    .HasDefaultValueSql("GETUTCDATE()");

                // Configurar IsActive como no nulo y con valor por defecto
                modelBuilder.Entity(entityType.Name).Property("IsActive")
                    .IsRequired()
                    .HasDefaultValue(true);

                // Hacer CreatedBy nullable
                modelBuilder.Entity(entityType.Name).Property("CreatedBy")
                    .IsRequired(false);

                // Hacer UpdatedAt nullable
                modelBuilder.Entity(entityType.Name).Property("UpdatedAt")
                    .IsRequired(false);

                // Hacer UpdatedBy nullable
                modelBuilder.Entity(entityType.Name).Property("UpdatedBy")
                    .IsRequired(false);
            }
        }
    }

    /// <summary>
    /// Aplica valores por defecto para campos de fecha
    /// </summary>
    public static void ApplyDateTimeConfiguration(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var properties = entityType.GetProperties()
                .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?));

            foreach (var property in properties)
            {
                modelBuilder.Entity(entityType.Name)
                    .Property(property.Name)
                    .HasColumnType("datetime2");
            }
        }
    }

    /// <summary>
    /// Aplica configuración de borrado suave a las entidades
    /// </summary>
    public static void ApplySoftDeleteQueryFilter(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, "IsActive");
                var condition = Expression.Equal(property, Expression.Constant(true));
                var lambda = Expression.Lambda(condition, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }
}