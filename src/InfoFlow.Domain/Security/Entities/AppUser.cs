using Microsoft.AspNetCore.Identity;

namespace InfoFlow.Domain.Security.Entities;

/// <summary>Usuário do sistema (Identity + claims/roles).</summary>
public class AppUser : IdentityUser<Guid>
{
  public string? FullName { get; set; }
  public bool IsActive { get; set; } = true;
}