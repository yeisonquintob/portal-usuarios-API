// Ruta: ./UserPortal.API/Extensions/ServiceCollectionExtensions.cs
using System;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using UserPortal.Business.Services.Implementations;
using UserPortal.Business.Services.Interfaces;
using UserPortal.Business.Validators;
using UserPortal.Data;
using UserPortal.Data.Infrastructure;
using UserPortal.Shared.DTOs.Request;

namespace UserPortal.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // AutoMapper
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        // Controllers con opciones de JSON
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        // Servicios de infraestructura
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

        // Servicios de negocio
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();

        // Validadores
        services.AddValidatorsFromAssemblyContaining<RegisterUserValidator>();
        services.AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters();

        // CORS
        ConfigureCors(services, configuration);

        // Caché
        services.AddMemoryCache(options =>
        {
            options.SizeLimit = 1024;
            options.CompactionPercentage = 0.25;
        });

        // Capa de datos
        services.AddDataLayer(configuration);

        // Health Checks básicos
        services.AddHealthChecks();

        return services;
    }

    private static void ConfigureCors(IServiceCollection services, IConfiguration configuration)
    {
        var origins = configuration.GetSection("Cors:Origins").Get<string[]>() ?? 
            new[] { "http://localhost:3000", "http://localhost:4200" };

        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigins", builder =>
            {
                builder
                    .WithOrigins(origins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithExposedHeaders("Token-Expired", "Token-Error");
            });
        });
    }
}