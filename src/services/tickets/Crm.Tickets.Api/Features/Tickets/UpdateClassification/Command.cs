using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.UpdateClassification;

/// <summary>SDD CRM-005 / specs/003-ticket-lifecycle.</summary>
public sealed record UpdateClassificationCommand(Guid Id, string Category, string Priority, string Actor)
    : IRequest<UpdateClassificationResponse>;
