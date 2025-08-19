using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using InfoFlow.Domain.Security.Entities;
using InfoFlow.Shared.Security.Authorization;
using InfoFlow.Shared.Security.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InfoFlow.Security.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
public class UsersController(
    UserManager<AppUser> userManager,
    RoleManager<AppRole> roleManager
) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = Permissions.SecurityUsersRead)]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50,
        CancellationToken ct = default)
    {
        take = Math.Clamp(take, 1, 200);

        var usersQuery = userManager.Users
            .AsNoTracking()
            .OrderBy(u => u.Email);

        var users = await usersQuery.Skip(skip).Take(take).ToListAsync(ct);

        var dtos = new List<UserDto>(users.Count);
        foreach (var u in users)
        {
            // RoleManager não lista roles por usuário; use o UserManager
            var full = await userManager.FindByIdAsync(u.Id.ToString());
            var roles = full is null
                ? Enumerable.Empty<string>()
                : await userManager.GetRolesAsync(full);

            dtos.Add(new UserDto(u.Id, u.Email!, u.FullName, u.IsActive, roles));
        }

        return Ok(dtos);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<ActionResult<UserDto>> Get(Guid id, CancellationToken ct)
    {
        var u = await userManager.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        if (u is null) return NotFound();

        var full = await userManager.FindByIdAsync(id.ToString());
        var roles = full is null ? Enumerable.Empty<string>() : await userManager.GetRolesAsync(full);

        return Ok(new UserDto(u.Id, u.Email!, u.FullName, u.IsActive, roles));
    }

    // ⬇⬇⬇ ESTE É O ENDPOINT QUE FALTAVA
    [HttpPost("{id:guid}/roles")]
    [Authorize(Policy = Permissions.SecurityRolesManage)]
    public async Task<IActionResult> AssignRole(Guid id, [FromBody] AssignRoleRequest input)
    {
        if (string.IsNullOrWhiteSpace(input.RoleName))
            return BadRequest("roleName é obrigatório.");

        var user = await userManager.FindByIdAsync(id.ToString());
        if (user is null) return NotFound($"Usuário {id} não encontrado.");

        var exists = await roleManager.RoleExistsAsync(input.RoleName);
        if (!exists) return NotFound($"Role '{input.RoleName}' não encontrada.");

        var already = await userManager.IsInRoleAsync(user, input.RoleName);
        if (already) return NoContent(); // idempotente

        var result = await userManager.AddToRoleAsync(user, input.RoleName);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException(errors);
        }

        return NoContent();
    }
}

// DTO que você já deve ter (ajuste o namespace se estiver em outro lugar)
public sealed record UserDto(Guid Id, string Email, string? FullName, bool IsActive, IEnumerable<string> Roles);