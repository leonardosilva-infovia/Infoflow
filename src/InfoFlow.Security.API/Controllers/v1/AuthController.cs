using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Asp.Versioning;
using InfoFlow.Application.Security.Abstractions;
using InfoFlow.Shared.Jwt;
using InfoFlow.Shared.Security.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfoFlow.Security.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController(
    IIdentityService identity,
    ITokenService tokens,
    IPermissionProvider permissionProvider,
    IRefreshTokenService refreshTokens,
    JwtOptions jwtOptions
) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest input)
    {
        var user = new InfoFlow.Domain.Security.Entities.AppUser
        {
            UserName = input.Email,
            Email = input.Email,
            FullName = input.FullName,
            IsActive = true
        };

        var result = await identity.CreateUserAsync(user, input.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors);
            return BadRequest(new ProblemDetails { Title = "Falha ao registrar", Detail = errors, Status = 400 });
        }

        var roles = await identity.GetUserRolesAsync(user);
        var perms = await permissionProvider.GetPermissionsAsync(user, roles);

        var tk = tokens.GenerateTokens(user.Id, user.UserName!, user.FullName, roles, perms);

        var ttl = TimeSpan.FromDays(jwtOptions.RefreshTokenLifetimeDays);
        var device = Request.Headers.UserAgent.ToString();
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var refresh = await refreshTokens.IssueAsync(user.Id, ttl, device, ip);

        return CreatedAtAction(nameof(Me), new { version = "1.0" },
            new TokenResponse(tk.AccessToken, tk.ExpiresAt, refresh, DateTime.UtcNow.Add(ttl)));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest input)
    {
        var user = await identity.FindByEmailAsync(input.Email);
        if (user is null || !user.IsActive) throw new UnauthorizedAccessException("Usuário inválido.");

        var ok = await identity.CheckPasswordAsync(user, input.Password);
        if (!ok) throw new UnauthorizedAccessException("Credenciais inválidas.");

        var roles = await identity.GetUserRolesAsync(user);
        var perms = await permissionProvider.GetPermissionsAsync(user, roles);

        var tk = tokens.GenerateTokens(user.Id, user.UserName!, user.FullName, roles, perms);

        var ttl = TimeSpan.FromDays(jwtOptions.RefreshTokenLifetimeDays);
        var device = Request.Headers.UserAgent.ToString();
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var refresh = await refreshTokens.IssueAsync(user.Id, ttl, device, ip);

        return Ok(new TokenResponse(tk.AccessToken, tk.ExpiresAt, refresh, DateTime.UtcNow.Add(ttl)));
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest input)
    {
      var validated = await refreshTokens.ValidateAsync(input.RefreshToken);

      var user = await identity.FindByIdAsync(validated.UserId)
                 ?? throw new UnauthorizedAccessException("Sessão inválida.");

      var roles = await identity.GetUserRolesAsync(user);
      var perms = await permissionProvider.GetPermissionsAsync(user, roles);

      var tk = tokens.GenerateTokens(user.Id, user.UserName!, user.FullName, roles, perms);

      var ttl = TimeSpan.FromDays(jwtOptions.RefreshTokenLifetimeDays);
      var device = validated.Device ?? null;
      var ip = validated.Ip ?? HttpContext.Connection.RemoteIpAddress?.ToString();

      var newRefresh = await refreshTokens.IssueAsync(user.Id, ttl, device, ip);

      // ⬇⬇⬇ rotação com vínculo do token novo
      await refreshTokens.RevokeAsync(input.RefreshToken, replacedByToken: newRefresh);

      return Ok(new TokenResponse(tk.AccessToken, tk.ExpiresAt, newRefresh, DateTime.UtcNow.Add(ttl)));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] RefreshRequest input)
    {
        await refreshTokens.RevokeAsync(input.RefreshToken);
        return NoContent();
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest input)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrWhiteSpace(userId)) throw new UnauthorizedAccessException("Token sem sub.");

        var user = await identity.FindByIdAsync(Guid.Parse(userId))
                   ?? throw new UnauthorizedAccessException("Usuário não encontrado.");

        var result = await identity.ChangePasswordAsync(user, input.CurrentPassword, input.NewPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors);
            return BadRequest(new ProblemDetails { Title = "Falha ao trocar senha", Detail = errors, Status = 400 });
        }

        return NoContent();
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        var user = await identity.FindByIdAsync(Guid.Parse(userId));
        if (user is null) return NotFound();

        var roles = await identity.GetUserRolesAsync(user);
        return Ok(new UserDto(user.Id, user.Email!, user.FullName, user.IsActive, roles));
    }
}