namespace Crm.Contracts.Tickets;

public sealed record TicketSummaryDto(
    string Id,
    string TicketNumber,
    string CustomerId,
    string CustomerName,
    string Subject,
    string Category,
    string Priority,
    string Status,
    string? AssignedAgentId,
    string? AssignedAgentName,
    bool IsEscalated);

public sealed record TicketDetailDto(
    string Id,
    string TicketNumber,
    string CustomerId,
    string CustomerName,
    string Subject,
    string? Description,
    string Category,
    string Priority,
    string Status,
    string? AssignedAgentId,
    string? AssignedAgentName,
    bool IsEscalated,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    IReadOnlyList<TicketHistoryDto> History);

public sealed record TicketHistoryDto(
    string Id,
    string Field,
    string? OldValue,
    string? NewValue,
    string ChangedBy,
    DateTimeOffset ChangedAt);

public sealed record CreateTicketRequest(
    string CustomerId,
    string CustomerName,
    string Subject,
    string? Description,
    string Category,
    string Priority);

public sealed record UpdateClassificationRequest(string Category, string Priority);

public sealed record AssignTicketRequest(string? AgentId, string? AgentName);

public sealed record ChangeStatusRequest(string Status);

public sealed record EscalateTicketRequest(string? AssignToAgentId, string? AssignToAgentName);

/// <summary>SDD CRM-018 / CRM-019 — result of applying SLA automation.</summary>
public sealed record RunAutomationResultDto(
    TicketSummaryDto Ticket,
    bool Assigned,
    bool Escalated,
    string? Message);

public sealed record TicketOptionsDto(
    IReadOnlyList<string> Categories,
    IReadOnlyList<string> Priorities,
    IReadOnlyList<string> Statuses,
    IReadOnlyList<AgentOptionDto> Agents);

public sealed record AgentOptionDto(string Id, string Name);
