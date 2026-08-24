using MediatR;

namespace Crm.Identity.Api.Features.Users.CreateUser;

/// <summary>SDD CRM-035.</summary>
public sealed record CreateUserCommand(
    string Email,
    string DisplayName,
    string Password,
    string Role) : IRequest<CreateUserResponse>;
