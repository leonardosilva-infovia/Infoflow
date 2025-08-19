using InfoFlow.Shared.Security.DTOs;
using MediatR;
using InfoFlow.Application.Security.Abstractions;

namespace InfoFlow.Application.Security.Queries;

public sealed record GetUserRolesQuery(Guid UserId) : IRequest<IReadOnlyCollection<string>>;

public class GetUserRolesHandler(IIdentityService identity)
  : IRequestHandler<GetUserRolesQuery, IReadOnlyCollection<string>>
{
  public async Task<IReadOnlyCollection<string>> Handle(GetUserRolesQuery request, CancellationToken ct)
  {
    var user = await identity.FindByIdAsync(request.UserId);
    if (user is null) throw new KeyNotFoundException("Usuário não encontrado.");
    var roles = await identity.GetUserRolesAsync(user);
    return roles.ToArray();
  }
}