using Crm.Contracts.Identity;
using Crm.Identity.Api.Domain;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.CreateUser;

/// <summary>SDD CRM-035.</summary>
public sealed record CreateUserCommand(
    string Email,
    string DisplayName,
    string Password,
    string Role) : IRequest<CreateUserResult>;

public sealed record CreateUserResult(UserSummaryDto? User, string? Error);

public sealed class CreateUserHandler(IdentityDb db) : IRequestHandler<CreateUserCommand, CreateUserResult>
{
    public Task<CreateUserResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (db.FindByEmail(request.Email) is not null)
            {
                return Task.FromResult(new CreateUserResult(null, "A user with that email already exists."));
            }

            var user = UserAccount.Register(request.Email, request.DisplayName, request.Password, request.Role);
            db.Insert(user);
            return Task.FromResult(new CreateUserResult(
                new UserSummaryDto(user.Id.ToString(), user.Email, user.DisplayName, user.Role, user.IsActive),
                null));
        }
        catch (Exception ex)
        {
            return Task.FromResult(new CreateUserResult(null, ex.Message));
        }
    }
}
