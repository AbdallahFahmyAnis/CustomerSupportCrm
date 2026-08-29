using Crm.Identity.Api.Domain;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.Auth.RegisterCustomer;

/// <summary>SDD CRM-045 / specs/identity/049-customer-register — force Customer role + issue tokens.</summary>
public sealed class RegisterCustomerHandler(IdentityDirectory directory)
    : IRequestHandler<RegisterCustomerCommand, RegisterCustomerResponse>
{
    public async Task<RegisterCustomerResponse> Handle(
        RegisterCustomerCommand request,
        CancellationToken cancellationToken)
    {
        var validation = RegisterCustomerValidator.Validate(request);
        if (validation is not null)
        {
            return new RegisterCustomerResponse(null, validation);
        }

        var (user, error) = await directory.CreateUserAsync(
            request.Email,
            request.DisplayName,
            request.Password,
            RoleNames.Customer,
            cancellationToken);
        if (error is not null || user is null)
        {
            return new RegisterCustomerResponse(null, error ?? "Registration failed.");
        }

        await directory.AppendAuditAsync(
            AuditActions.UserCreated,
            true,
            user.Id,
            user.Email,
            user.Id,
            user.Email,
            "SelfRegister Role=Customer",
            cancellationToken);

        var pair = await directory.IssuePairAsync(user, DateTimeOffset.UtcNow, cancellationToken);
        await directory.AppendAuditAsync(
            AuditActions.Login,
            true,
            user.Id,
            user.Email,
            user.Id,
            user.Email,
            "SelfRegister",
            cancellationToken);

        return new RegisterCustomerResponse(pair, null);
    }
}
