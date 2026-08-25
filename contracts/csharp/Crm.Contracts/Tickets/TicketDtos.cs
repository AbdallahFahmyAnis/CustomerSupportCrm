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
    IReadOnlyList<TicketHistoryDto> History,
    IReadOnlyList<TicketNoteDto> Notes);

/// <summary>SDD CRM-016 — internal agent note on a ticket.</summary>
public sealed record TicketNoteDto(
    string Id,
    string Body,
    string AuthorName,
    string? AuthorUserId,
    IReadOnlyList<string> MentionedUserIds,
    DateTimeOffset CreatedAt);

public sealed record AddTicketNoteRequest(string Body);

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

/// <summary>SDD CRM-014 — ticket follow-up task.</summary>
public sealed record TicketTaskDto(
    string Id,
    string TicketId,
    string Title,
    DateTimeOffset? DueAt,
    string? AssigneeUserId,
    string? AssigneeName,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record CreateTicketTaskRequest(
    string Title,
    DateTimeOffset? DueAt,
    string? AssigneeUserId,
    string? AssigneeName);
