using Crm.Contracts.Identity;

namespace Crm.Identity.Api.Features.Audit.ListAuditLogs;

public sealed record ListAuditLogsResponse(IReadOnlyList<AuditLogDto> Items);
