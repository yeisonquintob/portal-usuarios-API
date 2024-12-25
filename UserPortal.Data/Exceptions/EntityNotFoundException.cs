using System;

namespace UserPortal.Data.Exceptions;

/// <summary>
/// Excepción base para errores de base de datos
/// </summary>
public class DatabaseException : Exception
{
    public DatabaseException(string message) : base(message)
    {
    }

    public DatabaseException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}