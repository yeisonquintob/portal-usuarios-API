// Ruta: ./UserPortal.API/Extensions/ApplicationServicesExtensions.cs
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserPortal.Business.Services.Implementations;
using UserPortal.Business.Services.Interfaces;
using UserPortal.Business.Validators;
using UserPortal.Data;
using UserPortal.Shared.DTOs.Request;

namespace UserPortal.API.Extensions;

public static class ApplicationServicesExtensions
{
    public static IServiceCollection ConfigureApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // AutoMapper
        services.AddAutoMapper(typeof(Business.Mappings.AutoMapperProfile).Assembly);

        // Validators
        services.AddScoped<IValidator<RegisterUserDTO>, RegisterUserValidator>();
        services.AddScoped<IValidator<LoginUserDTO>, LoginValidator>();
        services.AddScoped<IValidator<UpdateUserDTO>, UpdateUserValidator>();

        // FluentValidation
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();

        // Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();

        // Data Layer
        services.AddDataLayer(configuration);

        // Cache
        services.AddMemoryCache();

        // CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigins",
                builder =>
                {
                    var origins = configuration.GetSection("Cors:Origins")
                        .Get<string[]>() ?? Array.Empty<string>();
                    
                    builder.WithOrigins(origins)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
        });

        return services;
    }
}