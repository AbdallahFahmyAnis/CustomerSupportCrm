using Crm.Contracts.Customers;
using Crm.Customers.Api.Infrastructure;
using MediatR;

namespace Crm.Customers.Api.Features.AddNote;

/// <summary>SDD CRM-003 / specs/002-customer-profiles.</summary>
public sealed record AddNoteCommand(Guid CustomerId, string Body, string AuthorName) : IRequest<AddNoteResult>;

public sealed record AddNoteResult(NoteDto? Note, string? Error);

public sealed class AddNoteHandler(CustomersDb db) : IRequestHandler<AddNoteCommand, AddNoteResult>
{
    public Task<AddNoteResult> Handle(AddNoteCommand request, CancellationToken cancellationToken)
    {
        var customer = db.GetCustomer(request.CustomerId);
        if (customer is null)
        {
            return Task.FromResult(new AddNoteResult(null, "Customer not found."));
        }

        try
        {
            var note = customer.AddNote(request.Body, request.AuthorName);
            db.InsertNote(note);
            return Task.FromResult(new AddNoteResult(
                new NoteDto(note.Id.ToString(), note.Body, note.AuthorName, note.CreatedAt),
                null));
        }
        catch (ArgumentException ex)
        {
            return Task.FromResult(new AddNoteResult(null, ex.Message));
        }
    }
}
