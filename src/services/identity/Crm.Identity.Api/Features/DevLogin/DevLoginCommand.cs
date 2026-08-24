using Crm.BuildingBlocks.Identity;
using Crm.Contracts.Identity;
using MediatR;

namespace Crm.Identity.Api.Features.DevLogin;

/// <summary>SDD 001-platform-foundation — demo login until CRM-035.</summary>
public sealed record DevLoginCommand(string Email, string Password) : IRequest<DevUserDto?>;

public sealed class DevLoginHandler : IRequestHandler<DevLoginCommand, DevUserDto?>
{
    public Task<DevUserDto?> Handle(DevLoginCommand request, CancellationToken cancellationToken)
    {
        var emailOk = string.Equals(request.Email, DevUsers.AgentEmail, StringComparison.OrdinalIgnoreCase);
        var passwordOk = request.Password == DevUsers.Password;
        if (!emailOk || !passwordOk)
        {
            return Task.FromResult<DevUserDto?>(null);
        }

        return Task.FromResult<DevUserDto?>(new DevUserDto(
            DevUsers.AgentId,
            DevUsers.AgentEmail,
            DevUsers.AgentName,
            DevUsers.AgentRole));
    }
}
