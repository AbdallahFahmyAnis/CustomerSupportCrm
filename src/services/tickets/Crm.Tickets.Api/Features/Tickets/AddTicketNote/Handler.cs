using Crm.Tickets.Api.Domain;
using Crm.Tickets.Api.Infrastructure;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.AddTicketNote;

/// <summary>SDD CRM-016 — add internal note and notify mentioned agents.</summary>
public sealed class AddTicketNoteHandler(
    TicketsDb db,
    NotificationsClient notifications) : IRequestHandler<AddTicketNoteCommand, AddTicketNoteResponse>
{
    public async Task<AddTicketNoteResponse> Handle(AddTicketNoteCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Body))
        {
            return new AddTicketNoteResponse(null, "Note body is required.");
        }

        var ticket = db.Get(request.TicketId);
        if (ticket is null)
        {
            return new AddTicketNoteResponse(null, "Ticket not found.");
        }

        var mentions = MentionParser.Parse(request.Body);
        var note = TicketNote.Create(
            ticket.Id,
            request.Body,
            request.AuthorName,
            request.AuthorUserId,
            mentions.Select(m => m.Id).ToList());
        db.InsertNote(note);

        foreach (var mention in mentions)
        {
            if (!string.IsNullOrWhiteSpace(request.AuthorUserId) &&
                mention.Id.Equals(request.AuthorUserId, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            await notifications.NotifyMentionAsync(
                mention.Id,
                ticket.TicketNumber,
                ticket.Id.ToString(),
                request.AuthorName,
                note.Body,
                cancellationToken);
        }

        return new AddTicketNoteResponse(
            new Crm.Contracts.Tickets.TicketNoteDto(
                note.Id.ToString(),
                note.Body,
                note.AuthorName,
                note.AuthorUserId,
                note.MentionedUserIds,
                note.CreatedAt),
            null);
    }
}
