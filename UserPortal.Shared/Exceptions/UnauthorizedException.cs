namespace UserPortal.Shared.Exceptions;

/// <summary>
/// Excepción que se lanza cuando un usuario no está autorizado para realizar una acción
/// </summary>
public class UnauthorizedException : Exception
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase UnauthorizedException
    /// </summary>
    public UnauthorizedException() : base() { }

    /// <summary>
    /// Inicializa una nueva instancia de la clase UnauthorizedException con un mensaje específico
    /// </summary>
    /// <param name="message">Mensaje que describe el error</param>
    public UnauthorizedException(string message) : base(message) { }

    /// <summary>
    /// Inicializa una nueva instancia de la clase UnauthorizedException con un mensaje y una excepción interna
    /// </summary>
    /// <param name="message">Mensaje que describe el error</param>
    /// <param name="innerException">Excepción que causó esta excepción</param>
    public UnauthorizedException(string message, Exception innerException) 
        : base(message, innerException) { }
}