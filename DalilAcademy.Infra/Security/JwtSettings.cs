namespace DalilAcademy.Infra.Security;

// Typed settings instead of magic strings. Key comes from user-secrets, never from appsettings.json.
public class JwtSettings
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public double ExpiresInHours { get; set; }
}
