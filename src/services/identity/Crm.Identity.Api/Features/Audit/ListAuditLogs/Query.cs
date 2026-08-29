using Crm.Contracts.Identity;
using MediatR;

namespace Crm.Identity.Api.Features.Audit.ListAuditLogs;

/// <summary>SDD CRM-036 / specs/051.</summary>
public sealed record ListAuditLogsQuery(
    string? Q,
    int Skip = 0,
    int Take = 25,
    string? Service = null) : IRequest<ListAuditLogsResponse>;

public sealed record ListAuditLogsResponse(AuditLogPageDto Page);
