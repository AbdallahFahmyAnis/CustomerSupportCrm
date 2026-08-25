using Crm.Contracts.Tickets;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.GetTicketReportSummary;

/// <summary>SDD CRM-031</summary>
public sealed record GetTicketReportSummaryQuery(DateTimeOffset? From, DateTimeOffset? To)
    : IRequest<TicketReportSummaryDto>;
