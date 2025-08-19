using InfoFlow.Shared.Security.DTOs;
using MediatR;
using InfoFlow.Domain.Security.Entities;
using Microsoft.AspNetCore.Identity;

namespace InfoFlow.Application.Security.Queries;

public sealed record GetRolesQuery() : IRequest<IReadOnlyCollection<RoleDto>>;

public class GetRolesHandler(RoleManager<AppRole> roles)
  : IRequestHandler<GetRolesQuery, IReadOnlyCollection<RoleDto>>
{
  public Task<IReadOnlyCollection<RoleDto>> Handle(GetRolesQuery request, CancellationToken ct)
  {
    // AppRole.Id já é Guid -> sem Guid.Parse
    var result = roles.Roles
      .Select(r => new RoleDto(r.Id, r.Name!))
      .ToArray();

    return Task.FromResult<IReadOnlyCollection<RoleDto>>(result);
  }
}