namespace InfoFlow.Domain.Security.Entities;

public class RefreshToken
{
  public Guid Id { get; set; } = Guid.NewGuid();
  public Guid UserId { get; set; }
  public string Token { get; set; } = default!;
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public string? CreatedByIp { get; set; }
  public DateTime ExpiresAt { get; set; }
  public DateTime? RevokedAt { get; set; }
  public string? ReplacedByToken { get; set; }

  public bool IsActive => RevokedAt == null && DateTime.UtcNow <= ExpiresAt;
}