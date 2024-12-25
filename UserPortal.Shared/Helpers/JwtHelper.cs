using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using UserPortal.Shared.Models;

namespace UserPortal.Shared.Helpers;

public static class JwtHelper
{
    // Variable estática para almacenar la configuración
    private static readonly string SecretKey = "tu_clave_secreta_muy_larga_y_segura_aqui_minimo_32_caracteres";
    private static readonly string Issuer = "UserPortalAPI";
    private static readonly string Audience = "UserPortalClient";
    private static readonly int ExpirationInMinutes = 60;

    public static string GenerateJwtToken(UserModel user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(SecretKey);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.RoleName ?? "User")
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(ExpirationInMinutes),
            Issuer = Issuer,
            Audience = Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public static ClaimsPrincipal? GetPrincipalFromToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(SecretKey);

        try
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = Issuer,
                ValidAudience = Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, 
                tokenValidationParameters, 
                out SecurityToken validatedToken);

            if (validatedToken is JwtSecurityToken jwtSecurityToken &&
                jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, 
                    StringComparison.InvariantCultureIgnoreCase))
            {
                return principal;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    public static bool ValidateToken(string token)
    {
        try
        {
            var principal = GetPrincipalFromToken(token);
            return principal != null;
        }
        catch
        {
            return false;
        }
    }

    public static string? GetUserIdFromToken(string token)
    {
        var principal = GetPrincipalFromToken(token);
        if (principal == null)
            return null;

        var claim = principal.FindFirst(ClaimTypes.NameIdentifier);
        return claim?.Value;
    }

    public static string? GetUserRoleFromToken(string token)
    {
        var principal = GetPrincipalFromToken(token);
        if (principal == null)
            return null;

        var claim = principal.FindFirst(ClaimTypes.Role);
        return claim?.Value;
    }

    public static bool IsTokenExpired(string token)
    {
        var principal = GetPrincipalFromToken(token);
        if (principal == null)
            return true;

        var expirationClaim = principal.FindFirst(JwtRegisteredClaimNames.Exp);
        if (expirationClaim == null)
            return true;

        var expiration = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expirationClaim.Value));
        return expiration.UtcDateTime <= DateTime.UtcNow;
    }

    public static IEnumerable<Claim>? GetTokenClaims(string token)
    {
        var principal = GetPrincipalFromToken(token);
        return principal?.Claims;
    }
}