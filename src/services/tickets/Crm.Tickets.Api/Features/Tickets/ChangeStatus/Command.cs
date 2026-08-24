using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.ChangeStatus;

/// <summary>SDD CRM-007 / specs/003-ticket-lifecycle.</summary>
public sealed record ChangeStatusCommand(Guid Id, string Status, string Actor) : IRequest<ChangeStatusResponse>;
