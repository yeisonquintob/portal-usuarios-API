namespace UserPortal.Data.Infrastructure;

/// <summary>
/// Implementación por defecto del proveedor de fecha y hora
/// </summary>
public class SystemDateTimeProvider : IDateTimeProvider
{
    /// <inheritdoc/>
    public DateTime UtcNow => DateTime.UtcNow;

    /// <inheritdoc/>
    public DateTime Now => DateTime.Now;
}
