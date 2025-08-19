using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using InfoFlow.Application.Security.Commands;
using InfoFlow.Application.Security.Queries;
using InfoFlow.Shared.Security.Authorization;
using InfoFlow.Shared.Security.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfoFlow.Security.API.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/roles")]
public class RolesController(IMediator mediator) : ControllerBase
{
    /// <summary>Lista todas as roles.</summary>
    [HttpGet]
    [Authorize(Policy = Permissions.SecurityRolesRead)]
    public async Task<ActionResult<IReadOnlyCollection<RoleDto>>> GetAll(CancellationToken ct)
    {
        var items = await mediator.Send(new GetRolesQuery(), ct);
        return Ok(items);
    }

    /// <summary>Cria uma nova role.</summary>
    [HttpPost]
    [Authorize(Policy = Permissions.SecurityRolesManage)]
    public async Task<ActionResult<RoleDto>> Create([FromBody] CreateRoleRequest input, CancellationToken ct)
    {
        var result = await mediator.Send(new CreateRoleCommand(input), ct);
        return CreatedAtAction(nameof(GetAll), new { version = "1.0" }, result);
    }

    /// <summary>Lista as roles de um usuário.</summary>
    [HttpGet("user/{userId:guid}")]
    [Authorize(Policy = Permissions.SecurityRolesRead)]
    public async Task<ActionResult<IReadOnlyCollection<string>>> GetUserRoles([FromRoute] Guid userId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetUserRolesQuery(userId), ct);
        return Ok(result);
    }

    /// <summary>Adiciona uma role ao usuário.</summary>
    [HttpPost("user")]
    [Authorize(Policy = Permissions.SecurityRolesManage)]
    public async Task<IActionResult> AddRoleToUser([FromBody] AddRoleToUserRequest input, CancellationToken ct)
    {
        await mediator.Send(new AddUserToRoleCommand(input.UserId, input.RoleName), ct);
        return NoContent();
    }

    /// <summary>Remove uma role do usuário.</summary>
    [HttpDelete("user")]
    [Authorize(Policy = Permissions.SecurityRolesManage)]
    public async Task<IActionResult> RemoveRoleFromUser([FromBody] RemoveRoleFromUserRequest input, CancellationToken ct)
    {
        await mediator.Send(new RemoveUserFromRoleCommand(input.UserId, input.RoleName), ct);
        return NoContent();
    }
}