namespace UserPortal.Shared.Constants;

/// <summary>
/// Mensajes de error estandarizados para toda la aplicación
/// </summary>
public static class ErrorMessages
{
    #region Authentication Errors
    public const string InvalidCredentials = "Las credenciales proporcionadas son incorrectas";
    public const string InvalidToken = "El token proporcionado es inválido o ha expirado";
    public const string InvalidRefreshToken = "El token de actualización es inválido o ha expirado";
    public const string UnauthorizedAccess = "No tiene permisos para realizar esta acción";
    public const string SessionExpired = "La sesión ha expirado, por favor inicie sesión nuevamente";
    #endregion

    #region User Errors
    public const string UserNotFound = "El usuario no fue encontrado";
    public const string EmailAlreadyExists = "El correo electrónico ya está registrado";
    public const string UsernameAlreadyExists = "El nombre de usuario ya está registrado";
    public const string InvalidEmail = "El correo electrónico proporcionado no es válido";
    public const string InvalidUsername = "El nombre de usuario debe tener entre 3 y 20 caracteres";
    public const string InvalidPassword = "La contraseña debe tener al menos 8 caracteres, incluir mayúsculas, minúsculas y números";
    public const string PasswordMismatch = "Las contraseñas no coinciden";
    public const string CurrentPasswordIncorrect = "La contraseña actual es incorrecta";
    #endregion

    #region Validation Errors
    public const string ValidationError = "Error de validación en los datos proporcionados";
    public const string RequiredField = "Este campo es requerido";
    public const string InvalidFormat = "El formato proporcionado no es válido";
    public const string InvalidLength = "La longitud del campo no es válida";
    #endregion

    #region Database Errors
    public const string DatabaseError = "Error al procesar la operación en la base de datos";
    public const string ConcurrencyError = "Los datos han sido modificados por otro usuario";
    public const string ConnectionError = "Error de conexión con la base de datos";
    #endregion

    #region File Errors
    public const string InvalidFileType = "El tipo de archivo no es válido";
    public const string FileTooLarge = "El archivo excede el tamaño máximo permitido";
    public const string FileUploadError = "Error al cargar el archivo";
    #endregion

    /// <summary>
    /// Formatea un mensaje de error con parámetros
    /// </summary>
    public static string Format(string message, params object[] args)
        => string.Format(message, args);

    /// <summary>
    /// Crea un mensaje de error de campo requerido
    /// </summary>
    public static string RequiredFieldError(string fieldName)
        => $"El campo {fieldName} es requerido";

    /// <summary>
    /// Crea un mensaje de error de longitud inválida
    /// </summary>
    public static string InvalidLengthError(string fieldName, int min, int max)
        => $"El campo {fieldName} debe tener entre {min} y {max} caracteres";
}