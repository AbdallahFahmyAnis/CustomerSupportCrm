using Crm.Contracts.Tickets;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.GetSlaPerformanceReport;

/// <summary>SDD CRM-032</summary>
public sealed record GetSlaPerformanceReportQuery(DateTimeOffset? From, DateTimeOffset? To)
    : IRequest<SlaPerformanceReportDto>;
