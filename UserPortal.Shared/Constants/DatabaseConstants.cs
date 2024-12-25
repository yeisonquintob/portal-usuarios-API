// Ruta: ./UserPortal.Shared/Constants/DatabaseConstants.cs
namespace UserPortal.Shared.Constants;

public static class DatabaseConstants
{
    public static class TableNames
    {
        public const string Users = "Users";
        public const string Roles = "Roles";
        public const string RefreshTokens = "RefreshTokens";
    }

    public static class FieldLengths
    {
        public const int Username = 50;
        public const int Email = 100;
        public const int Password = 256;
        public const int Name = 50;
        public const int Description = 200;
        public const int Token = 256;
        public const int ProfilePicture = 2048;
    }

    public static class DefaultValues
    {
        public const bool IsActive = true;
        public const string AdminRole = "Admin";
        public const string UserRole = "User";
    }
}