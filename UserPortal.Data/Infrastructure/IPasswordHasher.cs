namespace UserPortal.Data.Infrastructure;

/// <summary>
/// Interfaz para el servicio de hash de contraseñas
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Genera un hash de la contraseña
    /// </summary>
    /// <param name="password">Contraseña en texto plano</param>
    /// <returns>Hash de la contraseña</returns>
    string HashPassword(string password);

    /// <summary>
    /// Verifica si una contraseña coincide con su hash
    /// </summary>
    /// <param name="password">Contraseña en texto plano</param>
    /// <param name="hash">Hash almacenado</param>
    /// <returns>True si la contraseña es correcta</returns>
    bool VerifyPassword(string password, string hash);
}