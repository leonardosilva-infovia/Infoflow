namespace InfoFlow.Application.Security.Abstractions;

public interface IRefreshTokenService
{
  Task<string> IssueAsync(Guid userId, TimeSpan ttl, string? device = null, string? ip = null);
  Task<ValidatedRefresh> ValidateAsync(string refreshToken);

  /// <summary>Revoga um refresh token (idempotente).</summary>
  Task RevokeAsync(string refreshToken);

  /// <summary>
  /// Revoga um refresh token e registra qual o novo token que o substituiu (rotação).
  /// </summary>
  Task RevokeAsync(string refreshToken, string? replacedByToken);
}

public sealed record ValidatedRefresh(Guid UserId, DateTime ExpiresAt, string? Device, string? Ip);