using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using UserPortal.Shared.DTOs.Response;
using UserPortal.Shared.Exceptions;
using System.Collections.Generic;

namespace UserPortal.API.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no manejado: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        // Logs detallados para debugging
        Console.WriteLine($"Tipo de excepción: {exception.GetType().Name}");
        Console.WriteLine($"Mensaje de excepción: {exception.Message}");
        
        ApiResponse<object> result;
        
        switch (exception)
        {
            case ValidationException validationEx:
                response.StatusCode = StatusCodes.Status400BadRequest;
                result = ApiResponse<object>.CreateError(
                    validationEx.Message, 
                    validationEx.Errors);
                break;

            case UnauthorizedException:
                response.StatusCode = StatusCodes.Status401Unauthorized;
                result = ApiResponse<object>.CreateError("Las credenciales proporcionadas son incorrectas");
                break;

            case NotFoundException notFoundEx:
                response.StatusCode = StatusCodes.Status404NotFound;
                result = ApiResponse<object>.CreateError(notFoundEx.Message);
                break;

            case CustomException customEx:
                response.StatusCode = StatusCodes.Status400BadRequest;
                result = ApiResponse<object>.CreateError(customEx.Message);
                break;

            default:
                response.StatusCode = StatusCodes.Status500InternalServerError;
                result = ApiResponse<object>.CreateError("Ha ocurrido un error interno del servidor");
                break;
        }

        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await response.WriteAsJsonAsync(result, jsonOptions);
    }
}