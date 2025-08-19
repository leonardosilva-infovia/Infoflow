using InfoFlow.Application.Security.Abstractions;
using InfoFlow.Domain.Security.Entities;
using Microsoft.AspNetCore.Identity;

namespace InfoFlow.Infrastructure.Security;

public class IdentityService(
    UserManager<AppUser> userManager,
    RoleManager<AppRole> roleManager
) : IIdentityService
{
    public async Task<AppUser?> FindByEmailAsync(string email)
        => await userManager.FindByEmailAsync(email);

    public async Task<AppUser?> FindByIdAsync(Guid id)
        => await userManager.FindByIdAsync(id.ToString());

    public async Task<(bool Succeeded, IEnumerable<string> Errors)> CreateUserAsync(AppUser user, string password)
    {
        // garante UserName/Email confirmados de forma consistente
        if (string.IsNullOrWhiteSpace(user.UserName)) user.UserName = user.Email;
        var result = await userManager.CreateAsync(user, password);
        return (result.Succeeded, result.Errors.Select(e => e.Description));
    }

    public async Task<bool> CheckPasswordAsync(AppUser user, string password)
        => await userManager.CheckPasswordAsync(user, password);

    public async Task<(bool Succeeded, IEnumerable<string> Errors)> ChangePasswordAsync(AppUser user, string currentPassword, string newPassword)
    {
        var result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        return (result.Succeeded, result.Errors.Select(e => e.Description));
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(AppUser user)
        => await userManager.GetRolesAsync(user);

    public async Task<(bool Succeeded, IEnumerable<string> Errors)> CreateRoleAsync(string roleName)
    {
        if (await roleManager.RoleExistsAsync(roleName))
            return (true, Array.Empty<string>());

        var result = await roleManager.CreateAsync(new AppRole { Name = roleName });
        return (result.Succeeded, result.Errors.Select(e => e.Description));
    }

    public async Task<(bool Succeeded, IEnumerable<string> Errors)> AddUserToRoleAsync(AppUser user, string roleName)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var created = await roleManager.CreateAsync(new AppRole { Name = roleName });
            if (!created.Succeeded)
                return (false, created.Errors.Select(e => e.Description));
        }

        var result = await userManager.AddToRoleAsync(user, roleName);
        return (result.Succeeded, result.Errors.Select(e => e.Description));
    }
    
    public async Task<AppRole?> FindRoleByNameAsync(string roleName)
      => await roleManager.FindByNameAsync(roleName);
}