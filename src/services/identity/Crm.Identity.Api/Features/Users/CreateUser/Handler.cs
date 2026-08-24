using Crm.Contracts.Identity;
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

        var (user, error) = await directory.CreateUserAsync(
            request.Email, request.DisplayName, request.Password, request.Role, cancellationToken);
        if (error is not null || user is null)
        {
            return new CreateUserResponse(null, error ?? "Create failed.");
        }

        return new CreateUserResponse(
            new UserSummaryDto(user.Id.ToString(), user.Email, user.DisplayName, user.Role, user.IsActive),
            null);
    }
}
