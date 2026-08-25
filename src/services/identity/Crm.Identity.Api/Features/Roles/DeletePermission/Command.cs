using MediatR;

namespace Crm.Identity.Api.Features.Roles.DeletePermission;

/// <summary>SDD CRM-035 — remove permission from catalog and role claims.</summary>
public sealed record DeletePermissionCommand(string Name, Guid? ActorId)
    : IRequest<DeletePermissionResponse>;

public sealed record DeletePermissionResponse(string? Error);
