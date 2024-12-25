namespace UserPortal.Shared.Models;

/// <summary>
/// Parámetros para controlar la paginación de resultados
/// </summary>
public class PaginationParams
{
    /// <summary>
    /// Tamaño máximo permitido para una página
    /// </summary>
    public static readonly int MaxPageSize = 50;

    /// <summary>
    /// Tamaño mínimo permitido para una página
    /// </summary>
    public static readonly int MinPageSize = 1;

    /// <summary>
    /// Tamaño de página por defecto
    /// </summary>
    public static readonly int DefaultPageSize = 10;

    private int _pageSize = DefaultPageSize;
    private int _pageNumber = 1;

    /// <summary>
    /// Obtiene o establece el número de página (mínimo 1)
    /// </summary>
    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value < 1 ? 1 : value;
    }

    /// <summary>
    /// Obtiene o establece el tamaño de página (entre MinPageSize y MaxPageSize)
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = Math.Clamp(value, MinPageSize, MaxPageSize);
    }

    /// <summary>
    /// Calcula el número de registros a saltar para la paginación en BD
    /// </summary>
    public int Skip => (PageNumber - 1) * PageSize;

    /// <summary>
    /// Constructor con valores por defecto
    /// </summary>
    public PaginationParams()
    {
        PageNumber = 1;
        PageSize = DefaultPageSize;
    }

    /// <summary>
    /// Constructor con parámetros específicos
    /// </summary>
    public PaginationParams(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    /// <summary>
    /// Crea una instancia con valores por defecto
    /// </summary>
    public static PaginationParams Default() => new();

    /// <summary>
    /// Crea una instancia con el tamaño de página máximo
    /// </summary>
    public static PaginationParams MaxSize() => new(1, MaxPageSize);

    /// <summary>
    /// Valida que los parámetros de paginación sean correctos
    /// </summary>
    public bool IsValid() => PageNumber >= 1 && PageSize >= MinPageSize && PageSize <= MaxPageSize;

    /// <summary>
    /// Convierte a string para debugging
    /// </summary>
    public override string ToString() => $"Page {PageNumber}, Size {PageSize}";
}