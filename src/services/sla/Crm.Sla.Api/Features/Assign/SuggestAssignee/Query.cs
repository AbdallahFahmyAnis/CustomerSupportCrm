using Crm.Contracts.Sla;
using MediatR;

namespace Crm.Sla.Api.Features.Assign.SuggestAssignee;

/// <summary>SDD CRM-018 — suggest assignee for category/priority.</summary>
public sealed record SuggestAssigneeQuery(string Category, string Priority) : IRequest<SuggestAssigneeDto>;
