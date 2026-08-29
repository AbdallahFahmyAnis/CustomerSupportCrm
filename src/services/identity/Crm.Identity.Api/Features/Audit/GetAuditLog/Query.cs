using Crm.Contracts.Identity;
using MediatR;

namespace Crm.Identity.Api.Features.Audit.GetAuditLog;

/// <summary>SDD CRM-036 / specs/051.</summary>
public sealed record GetAuditLogQuery(Guid Id) : IRequest<AuditLogDetailDto?>;
