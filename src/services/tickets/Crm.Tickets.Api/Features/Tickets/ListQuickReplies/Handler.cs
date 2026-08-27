using Crm.Contracts.Tickets;
using Crm.Tickets.Api.Domain;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.ListQuickReplies;

/// <summary>SDD CRM-015</summary>
public sealed class ListQuickRepliesHandler : IRequestHandler<ListQuickRepliesQuery, IReadOnlyList<QuickReplyDto>>
{
    public Task<IReadOnlyList<QuickReplyDto>> Handle(ListQuickRepliesQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<QuickReplyDto> items = TicketCatalog.QuickReplies
            .Select(q => new QuickReplyDto(q.Id, q.Title, q.Body))
            .ToList();
        return Task.FromResult(items);
    }
}
