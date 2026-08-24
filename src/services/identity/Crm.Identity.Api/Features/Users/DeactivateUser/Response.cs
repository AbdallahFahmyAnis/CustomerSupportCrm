using Crm.Contracts.Identity;

namespace Crm.Identity.Api.Features.Users.DeactivateUser;

public sealed record DeactivateUserResponse(UserSummaryDto? User, string? Error);
