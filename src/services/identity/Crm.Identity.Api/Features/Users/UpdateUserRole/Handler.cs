using Crm.Contracts.Identity;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.Users.UpdateUserRole;

public sealed class UpdateUserRoleHandler(IdentityDirectory directory)
    : IRequestHandler<UpdateUserRoleCommand, UpdateUserRoleResponse>
{
    public async Task<UpdateUserRoleResponse> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        var validation = UpdateUserRoleValidator.Validate(request);
        if (validation is not null)
        {
            return new UpdateUserRoleResponse(null, validation);
        }

        try
        {
            var user = await directory.GetUserAsync(request.UserId, cancellationToken);
            if (user is null)
            {
                return new UpdateUserRoleResponse(null, "User not found.");
            }

            user.AssignRole(request.Role);
            await directory.UpdateAsync(user, cancellationToken);
            return new UpdateUserRoleResponse(
                new UserSummaryDto(user.Id.ToString(), user.Email, user.DisplayName, user.Role, user.IsActive),
                null);
        }
        catch (Exception ex)
        {
            return new UpdateUserRoleResponse(null, ex.Message);
        }
    }
}
