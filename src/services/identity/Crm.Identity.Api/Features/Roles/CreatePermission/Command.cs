using MediatR;

namespace Crm.Identity.Api.Features.Roles.CreatePermission;

/// <summary>SDD CRM-035 — add permission to catalog.</summary>
public sealed record CreatePermissionCommand(
    string Name,
    string? Description,
    Guid? ActorId) : IRequest<CreatePermissionResponse>;

public sealed record CreatePermissionResponse(string? Name, string? Error);
