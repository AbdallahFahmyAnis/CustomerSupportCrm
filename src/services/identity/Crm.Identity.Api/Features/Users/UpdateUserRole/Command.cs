using MediatR;

namespace Crm.Identity.Api.Features.Users.UpdateUserRole;

/// <summary>SDD CRM-035 / CRM-036.</summary>
public sealed record UpdateUserRoleCommand(Guid UserId, string Role, Guid? ActorId) : IRequest<UpdateUserRoleResponse>;
