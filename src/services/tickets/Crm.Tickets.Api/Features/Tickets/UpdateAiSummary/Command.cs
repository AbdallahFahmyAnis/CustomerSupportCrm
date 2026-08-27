using MediatR;

namespace Crm.Tickets.Api.Features.Tickets.UpdateAiSummary;

/// <summary>SDD CRM-023 polish / specs/042</summary>
public sealed record UpdateAiSummaryCommand(Guid Id, string Summary, IReadOnlyList<string>? Highlights)
    : IRequest<UpdateAiSummaryResponse>;
