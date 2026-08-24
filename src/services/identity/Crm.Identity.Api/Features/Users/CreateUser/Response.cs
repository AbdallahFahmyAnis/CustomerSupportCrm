using Crm.Contracts.Identity;

namespace Crm.Identity.Api.Features.Users.CreateUser;

public sealed record CreateUserResponse(UserSummaryDto? User, string? Error);
