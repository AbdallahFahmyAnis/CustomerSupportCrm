using Crm.Contracts.Tickets;
using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.SubmitTicketFeedback;

/// <summary>SDD CRM-030</summary>
public sealed record SubmitTicketFeedbackCommand(
    Guid? TicketId,
    string? TicketNumber,
    int Rating,
    string? Comment) : IRequest<SubmitTicketFeedbackResponse>;

public sealed record SubmitTicketFeedbackResponse(TicketFeedbackDto? Feedback, string? Error);
