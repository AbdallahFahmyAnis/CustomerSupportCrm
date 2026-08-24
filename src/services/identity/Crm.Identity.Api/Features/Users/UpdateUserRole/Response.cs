using Crm.Contracts.Identity;

namespace Crm.Identity.Api.Features.Users.UpdateUserRole;

public sealed record UpdateUserRoleResponse(UserSummaryDto? User, string? Error);
