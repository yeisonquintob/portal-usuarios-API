namespace UserPortal.Data.Infrastructure;

/// <summary>
/// Interfaz para proveer fechas y horas del sistema
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// Obtiene la fecha y hora actual en UTC
    /// </summary>
    DateTime UtcNow { get; }

    /// <summary>
    /// Obtiene la fecha y hora local actual
    /// </summary>
    DateTime Now { get; }
}
