using Crm.Contracts.Customers;

namespace Crm.Customers.Api.Features.Notes.AddNote;

public sealed record AddNoteResponse(NoteDto? Note, string? Error);
