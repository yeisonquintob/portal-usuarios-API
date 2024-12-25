namespace UserPortal.Shared.Constants;

/// <summary>
/// Constantes que definen los roles disponibles en el sistema
/// </summary>
public static class UserRoles
{
    /// <summary>
    /// Rol con acceso total al sistema
    /// </summary>
    public const string Admin = "Admin";

    /// <summary>
    /// Rol con acceso limitado al sistema
    /// </summary>
    public const string User = "User";

    /// <summary>
    /// Lista inmutable de todos los roles disponibles
    /// </summary>
    public static readonly IReadOnlyList<string> AllRoles = new[] { Admin, User };

    /// <summary>
    /// Verifica si un rol es válido en el sistema
    /// </summary>
    public static bool IsValidRole(string role) => 
        AllRoles.Contains(role, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Obtiene los roles disponibles como un array
    /// </summary>
    public static string[] GetAvailableRoles() => AllRoles.ToArray();

    /// <summary>
    /// Verifica si un rol es de tipo administrador
    /// </summary>
    public static bool IsAdmin(string role) => 
        string.Equals(role, Admin, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Verifica si un rol es de tipo usuario regular
    /// </summary>
    public static bool IsUser(string role) => 
        string.Equals(role, User, StringComparison.OrdinalIgnoreCase);
}