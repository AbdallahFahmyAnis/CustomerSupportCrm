using Crm.Contracts.Identity;
using Crm.Identity.Api.Features.IssueToken;
using MediatR;

namespace Crm.Identity.Api.Features.DevLogin;

/// <summary>SDD CRM-035 — backward-compatible login alias → token issuance.</summary>
public sealed record DevLoginCommand(string Email, string Password) : IRequest<TokenResponseDto?>;

public sealed class DevLoginHandler(IMediator mediator) : IRequestHandler<DevLoginCommand, TokenResponseDto?>
{
    public async Task<TokenResponseDto?> Handle(DevLoginCommand request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new IssueTokenCommand(request.Email, request.Password), cancellationToken);
        return result.Tokens;
    }
}
