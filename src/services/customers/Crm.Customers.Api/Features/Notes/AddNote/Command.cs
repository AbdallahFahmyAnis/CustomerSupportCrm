using MediatR;

namespace Crm.Customers.Api.Features.Notes.AddNote;

/// <summary>SDD CRM-003 / specs/002-customer-profiles.</summary>
public sealed record AddNoteCommand(Guid CustomerId, string Body, string AuthorName) : IRequest<AddNoteResponse>;
