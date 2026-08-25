using MediatR;

namespace Crm.Identity.Api.Features.Users.CreateUser;

/// <summary>SDD CRM-035 / CRM-036.</summary>
public sealed record CreateUserCommand(
    string Email,
    string DisplayName,
    string Password,
    string Role,
    Guid? ActorId) : IRequest<CreateUserResponse>;
