// Ruta: ./UserPortal.API/Program.cs
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using UserPortal.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/userportal-.txt", 
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7)
    .CreateLogger();

try
{
    builder.Host.UseSerilog();

    // Configurar servicios
    builder.Services
        .AddApplicationServices(builder.Configuration)
        .ConfigureJwtAuthentication(builder.Configuration)
        .AddSwaggerDocumentation();

    // Construir la aplicación
    var app = builder.Build();

    // Configurar el pipeline HTTP
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c => 
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "User Portal API v1");
            c.RoutePrefix = string.Empty;
        });
    }

    // Middleware de manejo de errores global
    app.UseErrorHandling();

    // Configuración de seguridad y routing
    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseCors("AllowSpecificOrigins");

    // Autenticación y autorización
    app.UseAuthentication();
    app.UseAuthorization();

    // Health checks
    app.MapHealthChecks("/health");

    // Endpoints de la API
    app.MapControllers();

    // Registro de las operaciones del middleware
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        options.GetLevel = (httpContext, elapsed, ex) => LogEventLevel.Information;
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        };
    });

    // Iniciar la aplicación
    Log.Information("Iniciando la aplicación User Portal API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "La aplicación terminó inesperadamente");
    throw;
}
finally
{
    Log.CloseAndFlush();
}