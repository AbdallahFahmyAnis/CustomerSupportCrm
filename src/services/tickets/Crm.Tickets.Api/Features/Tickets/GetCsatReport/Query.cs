using Crm.Contracts.Tickets;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.GetCsatReport;

/// <summary>SDD CRM-033</summary>
public sealed record GetCsatReportQuery(DateTimeOffset? From, DateTimeOffset? To)
    : IRequest<CsatReportDto>;
