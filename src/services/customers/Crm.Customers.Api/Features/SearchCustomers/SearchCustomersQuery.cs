using Crm.Contracts.Customers;
using Crm.Customers.Api.Infrastructure;
using MediatR;

namespace Crm.Customers.Api.Features.SearchCustomers;

/// <summary>SDD CRM-001 / specs/002-customer-profiles.</summary>
public sealed record SearchCustomersQuery(string? Q) : IRequest<IReadOnlyList<CustomerSummaryDto>>;

public sealed class SearchCustomersHandler(CustomersDb db)
    : IRequestHandler<SearchCustomersQuery, IReadOnlyList<CustomerSummaryDto>>
{
    public Task<IReadOnlyList<CustomerSummaryDto>> Handle(SearchCustomersQuery request, CancellationToken cancellationToken)
    {
        var rows = db.Search(request.Q)
            .Select(r => new CustomerSummaryDto(
                r.Id.ToString(),
                r.DisplayName,
                r.Organization,
                r.Status,
                r.UniqueIdentifier))
            .ToList();
        return Task.FromResult<IReadOnlyList<CustomerSummaryDto>>(rows);
    }
}
