using Crm.Contracts.Identity;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.Users.SearchUsers;

public sealed class SearchUsersHandler(IdentityDirectory directory)
    : IRequestHandler<SearchUsersQuery, IReadOnlyList<UserSummaryDto>>
{
    public async Task<IReadOnlyList<UserSummaryDto>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
    {
        var rows = await directory.SearchUsersAsync(request.Q, cancellationToken);
        return rows
            .Select(u => new UserSummaryDto(u.Id.ToString(), u.Email, u.DisplayName, u.Role, u.IsActive))
            .ToList();
    }
}
