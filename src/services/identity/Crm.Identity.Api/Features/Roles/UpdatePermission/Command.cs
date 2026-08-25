using MediatR;

namespace Crm.Identity.Api.Features.Roles.UpdatePermission;

/// <summary>SDD CRM-035 — rename / update permission catalog entry.</summary>
public sealed record UpdatePermissionCommand(
    string CurrentName,
    string Name,
    string? Description,
    Guid? ActorId) : IRequest<UpdatePermissionResponse>;

public sealed record UpdatePermissionResponse(string? Name, string? Error);
