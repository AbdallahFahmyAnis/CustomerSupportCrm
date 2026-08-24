using Crm.Contracts.Identity;
using Crm.Identity.Api.Features.Auth.IssueToken;
using MediatR;

namespace Crm.Identity.Api.Features.Auth.DevLogin;

public sealed class DevLoginHandler(IMediator mediator) : IRequestHandler<DevLoginCommand, TokenResponseDto?>
{
    public async Task<TokenResponseDto?> Handle(DevLoginCommand request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new IssueTokenCommand(request.Email, request.Password), cancellationToken);
        return result.Tokens;
    }
}
