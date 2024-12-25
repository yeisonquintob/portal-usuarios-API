using System.Text.Json.Serialization;

namespace UserPortal.Shared.DTOs.Response;

/// <summary>
/// Clase genérica para estandarizar todas las respuestas de la API
/// </summary>
/// <typeparam name="T">Tipo de datos que contendrá la respuesta</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Indica si la operación fue exitosa
    /// </summary>
    public bool IsSuccess { get; private set; }

    /// <summary>
    /// Mensaje descriptivo de la operación
    /// Puede ser null cuando no se requiere mensaje
    /// </summary>
    public string? Message { get; private set; }

    /// <summary>
    /// Datos de la respuesta, del tipo genérico T
    /// Puede ser null cuando no hay datos
    /// </summary>
    public T? Data { get; private set; }

    /// <summary>
    /// Diccionario de errores de validación
    /// Solo se incluye cuando hay errores
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IDictionary<string, string[]>? Errors { get; private set; }

    /// <summary>
    /// Constructor privado base
    /// </summary>
    private ApiResponse()
    {
    }

    /// <summary>
    /// Crea una respuesta exitosa sin datos
    /// </summary>
    /// <param name="message">Mensaje opcional de éxito</param>
    public static ApiResponse<T> CreateSuccess(string? message = null)
    {
        return new ApiResponse<T>
        {
            IsSuccess = true,
            Message = message,
            Data = default,
            Errors = null
        };
    }

    /// <summary>
    /// Crea una respuesta exitosa con datos
    /// </summary>
    /// <param name="data">Datos a incluir en la respuesta</param>
    /// <param name="message">Mensaje opcional de éxito</param>
    public static ApiResponse<T> CreateSuccess(T data, string? message = null)
    {
        return new ApiResponse<T>
        {
            IsSuccess = true,
            Message = message,
            Data = data,
            Errors = null
        };
    }

    /// <summary>
    /// Crea una respuesta de error
    /// </summary>
    /// <param name="message">Mensaje de error</param>
    /// <param name="errors">Diccionario opcional de errores de validación</param>
    public static ApiResponse<T> CreateError(string message, IDictionary<string, string[]>? errors = null)
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            Message = message,
            Data = default,
            Errors = errors
        };
    }
}