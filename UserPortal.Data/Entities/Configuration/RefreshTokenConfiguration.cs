using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserPortal.Data.Entities;

namespace UserPortal.Data.Entities.Configuration;

/// <summary>
/// Configuración de EF Core para la entidad RefreshToken
/// </summary>
public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        // Configuración de la tabla
        builder.ToTable("RefreshTokens");

        // Clave primaria
        builder.HasKey(rt => rt.Id);

        // Propiedades requeridas y sus restricciones
        builder.Property(rt => rt.Token)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(rt => rt.ExpiresAt)
            .IsRequired();

        // Índices
        builder.HasIndex(rt => rt.Token);
        builder.HasIndex(rt => new { rt.UserId, rt.InvalidatedAt, rt.UsedAt, rt.ExpiresAt });

        // Ignorar la propiedad computada
        builder.Ignore(rt => rt.IsValidToken);
    }
}