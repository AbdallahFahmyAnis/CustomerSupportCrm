using Crm.Contracts.Identity;
using Crm.Identity.Api.Infrastructure;
using MediatR;

namespace Crm.Identity.Api.Features.Users.SearchUsers;

public sealed class SearchUsersHandler(IdentityDirectory directory)
    : IRequestHandler<SearchUsersQuery, IReadOnlyList<UserSummaryDto>>
{
    public Task<IReadOnlyList<UserSummaryDto>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
        => directory.ListUserSummariesAsync(request.Q, cancellationToken);
}
