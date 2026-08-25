using Crm.Contracts.Tickets;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.AddTicketNote;

/// <summary>SDD CRM-016 — add internal note (+ optional @mentions).</summary>
public sealed record AddTicketNoteCommand(
    Guid TicketId,
    string Body,
    string AuthorName,
    string? AuthorUserId) : IRequest<AddTicketNoteResponse>;

public sealed record AddTicketNoteResponse(TicketNoteDto? Note, string? Error);
