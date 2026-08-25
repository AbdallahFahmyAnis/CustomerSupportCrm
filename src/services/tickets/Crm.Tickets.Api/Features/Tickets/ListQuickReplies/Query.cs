using Crm.Contracts.Tickets;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.ListQuickReplies;

/// <summary>SDD CRM-015</summary>
public sealed record ListQuickRepliesQuery : IRequest<IReadOnlyList<QuickReplyDto>>;
