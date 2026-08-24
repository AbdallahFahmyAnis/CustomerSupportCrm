using Crm.Contracts.Customers;
using Crm.Customers.Api.Infrastructure;
using MediatR;

namespace Crm.Customers.Api.Features.Notes.AddNote;

public sealed class AddNoteHandler(CustomersDb db) : IRequestHandler<AddNoteCommand, AddNoteResponse>
{
    public Task<AddNoteResponse> Handle(AddNoteCommand request, CancellationToken cancellationToken)
    {
        var customer = db.GetCustomer(request.CustomerId);
        if (customer is null)
        {
            return Task.FromResult(new AddNoteResponse(null, "Customer not found."));
        }

        try
        {
            var note = customer.AddNote(request.Body, request.AuthorName);
            db.InsertNote(note);
            return Task.FromResult(new AddNoteResponse(
                new NoteDto(note.Id.ToString(), note.Body, note.AuthorName, note.CreatedAt),
                null));
        }
        catch (ArgumentException ex)
        {
            return Task.FromResult(new AddNoteResponse(null, ex.Message));
        }
    }
}
