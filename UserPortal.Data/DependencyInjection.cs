using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserPortal.Data.Context;
using UserPortal.Data.Infrastructure;
using UserPortal.Data.Repositories.Implementations;
using UserPortal.Data.Repositories.Interfaces;
using UserPortal.Data.UnitOfWork;

namespace UserPortal.Data;

/// <summary>
/// Extensiones para la configuración de servicios de la capa de datos
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Configura los servicios de la capa de datos
    /// </summary>
    public static IServiceCollection AddDataLayer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configurar el contexto de base de datos
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                });

            #if DEBUG
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
            #endif
        });

        // Registrar infraestructura
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

        // Registrar repositorios
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();

        // Registrar Unit of Work
        services.AddScoped<IUnitOfWork, global::UserPortal.Data.UnitOfWork.UnitOfWork>();

        return services;
    }
}
