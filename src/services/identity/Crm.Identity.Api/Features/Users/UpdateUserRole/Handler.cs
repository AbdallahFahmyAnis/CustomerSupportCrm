using Crm.Contracts.Identity;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.Users.UpdateUserRole;

public sealed class UpdateUserRoleHandler(IdentityDb db) : IRequestHandler<UpdateUserRoleCommand, UpdateUserRoleResponse>
{
    public Task<UpdateUserRoleResponse> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        var validation = UpdateUserRoleValidator.Validate(request);
        if (validation is not null)
        {
            return Task.FromResult(new UpdateUserRoleResponse(null, validation));
        }

        try
        {
            var user = db.GetUser(request.UserId);
            if (user is null)
            {
                return Task.FromResult(new UpdateUserRoleResponse(null, "User not found."));
            }

            user.AssignRole(request.Role);
            db.Update(user);
            return Task.FromResult(new UpdateUserRoleResponse(
                new UserSummaryDto(user.Id.ToString(), user.Email, user.DisplayName, user.Role, user.IsActive),
                null));
        }
        catch (Exception ex)
        {
            return Task.FromResult(new UpdateUserRoleResponse(null, ex.Message));
        }
    }
}
