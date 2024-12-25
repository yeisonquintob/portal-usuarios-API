using System;
using System.Collections.Generic;

namespace UserPortal.Shared.Exceptions;

/// <summary>
/// Excepción que se lanza cuando ocurren errores de validación
/// </summary>
[Serializable]
public class ValidationException : Exception
{
    public IDictionary<string, string[]>? Errors { get; }

    public ValidationException() : base() { }

    public ValidationException(string message) : base(message) { }

    public ValidationException(string message, IDictionary<string, string[]> errors) 
        : base(message)
    {
        Errors = errors;
    }

    public ValidationException(string message, Exception innerException) 
        : base(message, innerException) { }
}