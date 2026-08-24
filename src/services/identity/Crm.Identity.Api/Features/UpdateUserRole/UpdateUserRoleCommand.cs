using Crm.Contracts.Identity;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.UpdateUserRole;

/// <summary>SDD CRM-035.</summary>
public sealed record UpdateUserRoleCommand(Guid UserId, string Role) : IRequest<UpdateUserRoleResult>;

public sealed record UpdateUserRoleResult(UserSummaryDto? User, string? Error);

public sealed class UpdateUserRoleHandler(IdentityDb db) : IRequestHandler<UpdateUserRoleCommand, UpdateUserRoleResult>
{
    public Task<UpdateUserRoleResult> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = db.GetUser(request.UserId);
            if (user is null)
            {
                return Task.FromResult(new UpdateUserRoleResult(null, "User not found."));
            }

            user.AssignRole(request.Role);
            db.Update(user);
            return Task.FromResult(new UpdateUserRoleResult(
                new UserSummaryDto(user.Id.ToString(), user.Email, user.DisplayName, user.Role, user.IsActive),
                null));
        }
        catch (Exception ex)
        {
            return Task.FromResult(new UpdateUserRoleResult(null, ex.Message));
        }
    }
}
