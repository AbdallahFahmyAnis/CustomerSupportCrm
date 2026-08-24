using Crm.Contracts.Tickets;
using Crm.Tickets.Api.Domain;
using MediatR;

namespace Crm.Tickets.Api.Features.GetTicketOptions;

/// <summary>SDD CRM-005 / specs/003-ticket-lifecycle.</summary>
public sealed record GetTicketOptionsQuery : IRequest<TicketOptionsDto>;

public sealed class GetTicketOptionsHandler : IRequestHandler<GetTicketOptionsQuery, TicketOptionsDto>
{
    public Task<TicketOptionsDto> Handle(GetTicketOptionsQuery request, CancellationToken cancellationToken)
        => Task.FromResult(new TicketOptionsDto(
            TicketCatalog.Categories,
            TicketCatalog.Priorities,
            TicketStatuses.All,
            TicketCatalog.Agents.Select(a => new AgentOptionDto(a.Id, a.Name)).ToList()));
}
