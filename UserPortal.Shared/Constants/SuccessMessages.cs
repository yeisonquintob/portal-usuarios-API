namespace UserPortal.Shared.Constants;

/// <summary>
/// Mensajes de éxito estandarizados para toda la aplicación
/// </summary>
public static class SuccessMessages
{
    #region Authentication Messages
    public const string LoginSuccess = "Inicio de sesión exitoso";
    public const string LogoutSuccess = "Sesión cerrada exitosamente";
    public const string PasswordResetSuccess = "Contraseña restablecida exitosamente";
    public const string EmailVerificationSuccess = "Correo electrónico verificado exitosamente";
    #endregion

    #region User Operations
    public const string UserCreated = "Usuario creado exitosamente";
    public const string UserUpdated = "Usuario actualizado exitosamente";
    public const string UserDeleted = "Usuario eliminado exitosamente";
    public const string ProfileUpdated = "Perfil actualizado exitosamente";
    public const string PasswordChanged = "Contraseña actualizada exitosamente";
    public const string EmailChanged = "Correo electrónico actualizado exitosamente";
    #endregion

    #region Profile Image
    public const string ProfileImageUploaded = "Imagen de perfil subida exitosamente";
    public const string ProfileImageDeleted = "Imagen de perfil eliminada exitosamente";
    public const string ProfileImageUpdated = "Imagen de perfil actualizada exitosamente";
    #endregion

    /// <summary>
    /// Formatea un mensaje de éxito con parámetros
    /// </summary>
    public static string Format(string message, params object[] args)
        => string.Format(message, args);

    /// <summary>
    /// Crea un mensaje de creación exitosa
    /// </summary>
    public static string Created(string entityName)
        => $"{entityName} creado exitosamente";

    /// <summary>
    /// Crea un mensaje de actualización exitosa
    /// </summary>
    public static string Updated(string entityName)
        => $"{entityName} actualizado exitosamente";

    /// <summary>
    /// Crea un mensaje de eliminación exitosa
    /// </summary>
    public static string Deleted(string entityName)
        => $"{entityName} eliminado exitosamente";
}