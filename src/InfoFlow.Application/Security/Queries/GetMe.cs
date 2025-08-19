using InfoFlow.Application.Security.Abstractions;
using InfoFlow.Shared.Security.DTOs;
using MediatR;

namespace InfoFlow.Application.Security.Queries;

public record GetMeQuery(Guid UserId) : IRequest<UserDto>;

public class GetMeHandler(IIdentityService identity) : IRequestHandler<GetMeQuery, UserDto>
{
  public async Task<UserDto> Handle(GetMeQuery request, CancellationToken ct)
  {
    var user = await identity.FindByIdAsync(request.UserId) ?? throw new KeyNotFoundException("Usuário não encontrado.");
    var roles = await identity.GetUserRolesAsync(user);
    return new UserDto(user.Id, user.Email!, user.FullName, user.IsActive, roles);
  }
}