using Crm.Contracts.Customers;
using Crm.Customers.Api.Domain;
using Crm.Customers.Api.Infrastructure;
using MediatR;

namespace Crm.Customers.Api.Features.Customers.GetCustomer;

public sealed class GetCustomerHandler(CustomersDb db) : IRequestHandler<GetCustomerQuery, CustomerDetailDto?>
{
    public Task<CustomerDetailDto?> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
    {
        var customer = db.GetCustomer(request.Id);
        return Task.FromResult(customer is null ? null : Map(customer));
    }

    public static CustomerDetailDto Map(Customer customer)
    {
        var notes = customer.Notes
            .Select(n => new NoteDto(n.Id.ToString(), n.Body, n.AuthorName, n.CreatedAt))
            .ToList();
        var attachments = customer.Attachments
            .Select(a => new AttachmentDto(a.Id.ToString(), a.FileName, a.ContentType, a.SizeBytes, a.CreatedAt))
            .ToList();
        var timeline = BuildTimeline(customer);
        return new CustomerDetailDto(
            customer.Id.ToString(),
            customer.DisplayName,
            customer.Organization,
            customer.Status,
            customer.UniqueIdentifier,
            customer.Contacts
                .Select(c => new ContactDto(c.Id.ToString(), c.Type, c.Value, c.IsPrimary, c.IsActive))
                .ToList(),
            notes,
            attachments,
            timeline);
    }

    private static IReadOnlyList<TimelineItemDto> BuildTimeline(Customer customer)
    {
        var items = new List<TimelineItemDto>();
        items.AddRange(customer.Notes.Select(n =>
            new TimelineItemDto(n.Id.ToString(), "note", n.Body, n.CreatedAt)));
        items.AddRange(customer.Attachments.Select(a =>
            new TimelineItemDto(a.Id.ToString(), "attachment", a.FileName, a.CreatedAt)));
        return items.OrderByDescending(i => i.OccurredAt).ToList();
    }
}
