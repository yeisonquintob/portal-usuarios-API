using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserPortal.Data.Entities;

namespace UserPortal.Data.Entities.Configuration;

/// <summary>
/// Configuración de EF Core para la entidad Role
/// </summary>
public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        // Configuración de la tabla
        builder.ToTable("Roles");

        // Clave primaria
        builder.HasKey(r => r.Id);

        // Propiedades requeridas y sus restricciones
        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.Description)
            .HasMaxLength(200);

        // Índice único para el nombre del rol
        builder.HasIndex(r => r.Name).IsUnique();

        // Datos semilla para roles predeterminados
        builder.HasData(
            new Role 
            { 
                Id = 1, 
                Name = "Admin", 
                Description = "Administrador del sistema",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new Role 
            { 
                Id = 2, 
                Name = "User", 
                Description = "Usuario regular del sistema",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            }
        );
    }
}