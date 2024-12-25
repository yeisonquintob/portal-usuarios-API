// Ruta: ./UserPortal.API/Extensions/ExceptionMiddlewareExtensions.cs
using Microsoft.AspNetCore.Builder;
using UserPortal.API.Middleware;

namespace UserPortal.API.Extensions;

public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseErrorHandling(
        this IApplicationBuilder app)
    {
        return app.UseMiddleware<ErrorHandlingMiddleware>();
    }
}