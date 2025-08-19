using InfoFlow.Domain.Security.Entities;

namespace InfoFlow.Application.Security.Abstractions;

public interface IIdentityService
{
  Task<AppUser?> FindByEmailAsync(string email);
  Task<AppUser?> FindByIdAsync(Guid id);
  Task<(bool Succeeded, IEnumerable<string> Errors)> CreateUserAsync(AppUser user, string password);
  Task<bool> CheckPasswordAsync(AppUser user, string password);
  Task<(bool Succeeded, IEnumerable<string> Errors)> ChangePasswordAsync(AppUser user, string currentPassword, string newPassword);
  Task<IEnumerable<string>> GetUserRolesAsync(AppUser user);
  Task<(bool Succeeded, IEnumerable<string> Errors)> CreateRoleAsync(string roleName);
  Task<(bool Succeeded, IEnumerable<string> Errors)> AddUserToRoleAsync(AppUser user, string roleName);
  Task<AppRole?> FindRoleByNameAsync(string roleName);
}