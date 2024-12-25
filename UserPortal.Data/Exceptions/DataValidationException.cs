using System;
using System.Collections.Generic;

namespace UserPortal.Data.Exceptions;

/// <summary>
/// Excepción para errores de validación de datos
/// </summary>
public class DataValidationException : DatabaseException
{
    public IDictionary<string, string[]> Errors { get; }

    public DataValidationException(string message, IDictionary<string, string[]> errors) 
        : base(message)
    {
        Errors = errors;
    }
}