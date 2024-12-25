using System;

namespace UserPortal.Shared.Exceptions;

/// <summary>
/// Excepción que se lanza cuando no se encuentra un recurso solicitado
/// </summary>
[Serializable]
public class NotFoundException : Exception
{
    /// <summary>
    /// Obtiene el nombre del recurso que no se encontró
    /// </summary>
    public string? ResourceName { get; }

    /// <summary>
    /// Obtiene el identificador del recurso que no se encontró
    /// </summary>
    public object? ResourceId { get; }

    public NotFoundException() : base() { }

    public NotFoundException(string message) : base(message) { }

    public NotFoundException(string resourceName, object resourceId)
        : base($"Resource '{resourceName}' with identifier '{resourceId}' was not found.")
    {
        ResourceName = resourceName;
        ResourceId = resourceId;
    }

    public NotFoundException(string message, Exception innerException) 
        : base(message, innerException) { }
}