// Shared/Jwt/JwtOptions.cs
namespace InfoFlow.Shared.Jwt;

public class JwtOptions
{
  public string Issuer { get; set; } = "infoflow-auth";
  public string Audience { get; set; } = "infoflow-api";
  public string Key { get; set; } = default!; // >= 32 chars
  public int AccessTokenLifetimeMinutes { get; set; } = 60;
  public int RefreshTokenLifetimeDays { get; set; } = 7;
}