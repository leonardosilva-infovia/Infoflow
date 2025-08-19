using System.Security.Claims;

namespace InfoFlow.Application.Security.Abstractions;

public record GeneratedToken(string AccessToken, DateTime ExpiresAt, string RefreshToken, DateTime RefreshExpiresAt);

public interface ITokenService
{
  GeneratedToken GenerateTokens(
    Guid userId,
    string userName,
    string? fullName,
    IEnumerable<string> roles,
    IEnumerable<string> permissions,
    IEnumerable<Claim>? extraClaims = null);
}