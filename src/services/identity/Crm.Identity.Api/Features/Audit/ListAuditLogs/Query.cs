using MediatR;

namespace Crm.Identity.Api.Features.Audit.ListAuditLogs;

/// <summary>SDD CRM-036.</summary>
public sealed record ListAuditLogsQuery(string? Q, int Take = 100) : IRequest<ListAuditLogsResponse>;
