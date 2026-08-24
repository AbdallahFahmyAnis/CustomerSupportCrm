using Crm.Contracts.Identity;
using Crm.Identity.Api.Domain;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.Users.CreateUser;

public sealed class CreateUserHandler(IdentityDb db) : IRequestHandler<CreateUserCommand, CreateUserResponse>
{
    public Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var validation = CreateUserValidator.Validate(request);
        if (validation is not null)
        {
            return Task.FromResult(new CreateUserResponse(null, validation));
        }

        try
        {
            if (db.FindByEmail(request.Email) is not null)
            {
                return Task.FromResult(new CreateUserResponse(null, "A user with that email already exists."));
            }

            var user = UserAccount.Register(request.Email, request.DisplayName, request.Password, request.Role);
            db.Insert(user);
            return Task.FromResult(new CreateUserResponse(
                new UserSummaryDto(user.Id.ToString(), user.Email, user.DisplayName, user.Role, user.IsActive),
                null));
        }
        catch (Exception ex)
        {
            return Task.FromResult(new CreateUserResponse(null, ex.Message));
        }
    }
}
