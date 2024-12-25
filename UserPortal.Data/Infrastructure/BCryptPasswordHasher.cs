using BCrypt.Net;
using Microsoft.Extensions.Logging;

namespace UserPortal.Data.Infrastructure;

public class BCryptPasswordHasher : IPasswordHasher
{
    private readonly ILogger<BCryptPasswordHasher> _logger;
    private const int WORK_FACTOR = 11;

    public BCryptPasswordHasher(ILogger<BCryptPasswordHasher> logger)
    {
        _logger = logger;
    }

    public string HashPassword(string password)
    {
        try
        {
            string salt = BCrypt.Net.BCrypt.GenerateSalt(WORK_FACTOR);
            _logger.LogDebug("Salt generado: {Salt}", salt);
            
            var hash = BCrypt.Net.BCrypt.HashPassword(password, salt);
            _logger.LogDebug("Hash generado: {Hash}", hash);
            
            return hash;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar hash de contraseña");
            throw;
        }
    }

    public bool VerifyPassword(string password, string storedHash)
    {
        try
        {
            _logger.LogDebug("Intentando verificar contraseña");
            _logger.LogDebug("Contraseña proporcionada length: {Length}", password.Length);
            _logger.LogDebug("Hash almacenado: {Hash}", storedHash);

            // Intentar verificación directa primero
            bool isValid = BCrypt.Net.BCrypt.Verify(password, storedHash);
            _logger.LogDebug("Resultado de verificación directa: {Result}", isValid);

            if (!isValid)
            {
                // Generar un nuevo hash con la misma contraseña para comparación
                var newHash = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(WORK_FACTOR));
                _logger.LogDebug("Nuevo hash generado para comparación: {Hash}", newHash);
                isValid = storedHash == newHash;
                _logger.LogDebug("Resultado de comparación con nuevo hash: {Result}", isValid);
            }

            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar contraseña");
            return false;
        }
    }
}