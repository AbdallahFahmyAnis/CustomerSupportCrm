using Crm.Contracts.Identity;
using MediatR;

namespace Crm.Identity.Api.Features.Users.SearchUsers;

/// <summary>SDD CRM-035.</summary>
public sealed record SearchUsersQuery(string? Q) : IRequest<IReadOnlyList<UserSummaryDto>>;
