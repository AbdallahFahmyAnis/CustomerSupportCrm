using Crm.Contracts.Identity;
using Crm.Identity.Api.Domain;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.Users.CreateUser;

public sealed class CreateUserHandler(IdentityDirectory directory)
    : IRequestHandler<CreateUserCommand, CreateUserResponse>
{
    public async Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var validation = CreateUserValidator.Validate(request);
        if (validation is not null)
        {
            return new CreateUserResponse(null, validation);
        }

        Guid? deptId = null;
        Guid? branchId = null;
        if (!string.IsNullOrWhiteSpace(request.DepartmentId) && Guid.TryParse(request.DepartmentId, out var d))
        {
            deptId = d;
        }

        if (!string.IsNullOrWhiteSpace(request.BranchId) && Guid.TryParse(request.BranchId, out var b))
        {
            branchId = b;
        }

        var (user, error) = await directory.CreateUserAsync(
            request.Email,
            request.DisplayName,
            request.Password,
            request.Role,
            cancellationToken,
            deptId,
            branchId);
        if (error is not null || user is null)
        {
            return new CreateUserResponse(null, error ?? "Create failed.");
        }

        string? actorEmail = null;
        if (request.ActorId is { } actorId)
        {
            var actor = await directory.GetUserAsync(actorId, cancellationToken);
            actorEmail = actor?.Email;
        }

        await directory.AppendAuditAsync(
            AuditActions.UserCreated,
            true,
            request.ActorId,
            actorEmail,
            user.Id,
            user.Email,
            $"Role={user.Role}",
            cancellationToken);

        return new CreateUserResponse(
            new UserSummaryDto(
                user.Id.ToString(),
                user.Email,
                user.DisplayName,
                user.Role,
                user.IsActive,
                deptId?.ToString(),
                branchId?.ToString()),
            null);
    }
}
