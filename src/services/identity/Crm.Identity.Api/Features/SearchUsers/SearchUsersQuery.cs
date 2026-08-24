using Crm.Contracts.Identity;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.SearchUsers;

/// <summary>SDD CRM-035.</summary>
public sealed record SearchUsersQuery(string? Q) : IRequest<IReadOnlyList<UserSummaryDto>>;

public sealed class SearchUsersHandler(IdentityDb db) : IRequestHandler<SearchUsersQuery, IReadOnlyList<UserSummaryDto>>
{
    public Task<IReadOnlyList<UserSummaryDto>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
    {
        var rows = db.SearchUsers(request.Q)
            .Select(u => new UserSummaryDto(u.Id.ToString(), u.Email, u.DisplayName, u.Role, u.IsActive))
            .ToList();
        return Task.FromResult<IReadOnlyList<UserSummaryDto>>(rows);
    }
}
