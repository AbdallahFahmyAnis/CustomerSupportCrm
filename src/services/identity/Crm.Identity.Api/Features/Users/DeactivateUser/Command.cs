using MediatR;

namespace Crm.Identity.Api.Features.Users.DeactivateUser;

/// <summary>SDD CRM-035.</summary>
public sealed record DeactivateUserCommand(Guid UserId, Guid? ActorId) : IRequest<DeactivateUserResponse>;
