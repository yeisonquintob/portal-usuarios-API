using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserPortal.Data.Entities;
using UserPortal.Data.Entities.Base;
using UserPortal.Data.Entities.Configuration;
using UserPortal.Data.Extensions;

namespace UserPortal.Data.Context;

/// <summary>
/// Contexto principal de la base de datos
/// </summary>
public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// Constructor que recibe las opciones de configuración
    /// </summary>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    #region DbSet Properties
    /// <summary>
    /// Usuarios del sistema
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// Roles del sistema
    /// </summary>
    public DbSet<Role> Roles => Set<Role>();

    /// <summary>
    /// Tokens de actualización
    /// </summary>
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    #endregion

    /// <summary>
    /// Configura el modelo de la base de datos
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Aplicar configuraciones globales
        modelBuilder.ApplyAuditableConfiguration();
        modelBuilder.ApplyDateTimeConfiguration();
        modelBuilder.ApplySoftDeleteQueryFilter();

        // Aplicar configuraciones específicas de entidades
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());

        // Configuración adicional de relaciones y restricciones
        ConfigureRelationships(modelBuilder);
        ConfigureDeleteBehavior(modelBuilder);
        ConfigureIndexes(modelBuilder);
    }

    /// <summary>
    /// Configura las relaciones entre entidades
    /// </summary>
    private void ConfigureRelationships(ModelBuilder modelBuilder)
    {
        // Usuario - Rol (Muchos a Uno)
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Usuario - RefreshTokens (Uno a Muchos)
        modelBuilder.Entity<User>()
            .HasMany(u => u.RefreshTokens)
            .WithOne(rt => rt.User)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    /// <summary>
    /// Configura el comportamiento de eliminación
    /// </summary>
    private void ConfigureDeleteBehavior(ModelBuilder modelBuilder)
    {
        // Configurar eliminación en cascada para RefreshTokens
        modelBuilder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .OnDelete(DeleteBehavior.Cascade);

        // Prevenir eliminación en cascada para Roles
        modelBuilder.Entity<Role>()
            .HasMany(r => r.Users)
            .WithOne(u => u.Role)
            .OnDelete(DeleteBehavior.Restrict);
    }

    /// <summary>
    /// Configura índices adicionales
    /// </summary>
    private void ConfigureIndexes(ModelBuilder modelBuilder)
    {
        // Índices para User
        modelBuilder.Entity<User>()
            .HasIndex(u => new { u.Username, u.Email })
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.LastLogin);

        // Índices para Role
        modelBuilder.Entity<Role>()
            .HasIndex(r => r.Name)
            .IsUnique();

        // Índices para RefreshToken
        modelBuilder.Entity<RefreshToken>()
            .HasIndex(rt => new { rt.Token, rt.ExpiresAt });

        modelBuilder.Entity<RefreshToken>()
            .HasIndex(rt => rt.UserId);
    }

    /// <summary>
    /// Configura opciones adicionales del contexto
    /// </summary>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Configuración por defecto si no se proporciona ninguna
            optionsBuilder.UseSqlServer("Name=DefaultConnection");
        }

        // Habilitar el seguimiento detallado solo en desarrollo
        #if DEBUG
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.EnableDetailedErrors();
        #endif
    }

    /// <summary>
    /// Sobreescribe SaveChanges para manejar auditoría
    /// </summary>
    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    /// <summary>
    /// Sobreescribe SaveChangesAsync para manejar auditoría
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Actualiza los campos de auditoría antes de guardar cambios
    /// </summary>
    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.IsActive = true;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    // No modificar CreatedAt ni CreatedBy en actualizaciones
                    entry.Property(x => x.CreatedAt).IsModified = false;
                    entry.Property(x => x.CreatedBy).IsModified = false;
                    break;

                case EntityState.Deleted:
                    // Convertir eliminación física en lógica
                    entry.State = EntityState.Modified;
                    entry.Entity.IsActive = false;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }
    }
}