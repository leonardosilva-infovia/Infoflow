using Microsoft.AspNetCore.Identity;

namespace InfoFlow.Domain.Security.Entities;

/// <summary>Role do sistema; permissões virão via claims na role.</summary>
public class AppRole : IdentityRole<Guid>
{
  public AppRole() {}
  public AppRole(string roleName) : base(roleName) {}
}