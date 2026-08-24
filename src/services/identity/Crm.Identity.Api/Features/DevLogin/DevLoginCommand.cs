using Crm.Contracts.Identity;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.DevLogin;

/// <summary>SDD CRM-035 — authenticate against Identity store (gateway BFF login).</summary>
public sealed record DevLoginCommand(string Email, string Password) : IRequest<DevUserDto?>;

public sealed class DevLoginHandler(IdentityDb db) : IRequestHandler<DevLoginCommand, DevUserDto?>
{
    public Task<DevUserDto?> Handle(DevLoginCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return Task.FromResult<DevUserDto?>(null);
        }

        var user = db.FindByEmail(request.Email);
        if (user is null || !user.VerifyPassword(request.Password))
        {
            return Task.FromResult<DevUserDto?>(null);
        }

        return Task.FromResult<DevUserDto?>(new DevUserDto(
            user.Id.ToString(),
            user.Email,
            user.DisplayName,
            user.Role));
    }
}
