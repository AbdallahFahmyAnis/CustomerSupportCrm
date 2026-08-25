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
    bool IsEscalated,
    string? DepartmentId = null);

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
    IReadOnlyList<TicketNoteDto> Notes,
    TicketFeedbackDto? Feedback = null,
    string? DepartmentId = null);

/// <summary>SDD CRM-016 — internal agent note on a ticket.</summary>
public sealed record TicketNoteDto(
    string Id,
    string Body,
    string AuthorName,
    string? AuthorUserId,
    IReadOnlyList<string> MentionedUserIds,
    DateTimeOffset CreatedAt);

public sealed record AddTicketNoteRequest(string Body);

/// <summary>SDD CRM-030 — customer CSAT on a ticket.</summary>
public sealed record TicketFeedbackDto(
    string Id,
    string TicketId,
    int Rating,
    string? Comment,
    DateTimeOffset CreatedAt);

public sealed record SubmitTicketFeedbackRequest(
    string? TicketId,
    string? TicketNumber,
    int Rating,
    string? Comment);

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
    string Priority,
    string? DepartmentId = null);

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

/// <summary>SDD CRM-015 — shared canned reply.</summary>
public sealed record QuickReplyDto(string Id, string Title, string Body);

/// <summary>SDD CRM-031 — ticket volume report.</summary>
public sealed record TicketReportSummaryDto(
    DateTimeOffset From,
    DateTimeOffset To,
    int Created,
    int Open,
    int ResolvedOrClosed,
    int Escalated,
    IReadOnlyList<ReportBucketDto> ByStatus,
    IReadOnlyList<ReportBucketDto> ByCategory,
    IReadOnlyList<ReportBucketDto> ByPriority,
    IReadOnlyList<ReportAgentBucketDto> ByAgent);

public sealed record ReportBucketDto(string Key, int Count);

public sealed record ReportAgentBucketDto(string? AgentId, string? AgentName, int Count);

/// <summary>SDD CRM-032 — SLA / agent performance.</summary>
public sealed record SlaPerformanceReportDto(
    DateTimeOffset From,
    DateTimeOffset To,
    int TicketCount,
    int ResolutionBreached,
    double BreachPercent,
    IReadOnlyList<SlaAgentPerformanceDto> ByAgent);

public sealed record SlaAgentPerformanceDto(
    string? AgentId,
    string? AgentName,
    int TicketCount,
    int ResolutionBreached);

/// <summary>SDD CRM-033 — CSAT aggregate report.</summary>
public sealed record CsatReportDto(
    DateTimeOffset From,
    DateTimeOffset To,
    int Count,
    double AverageRating,
    IReadOnlyList<CsatDistributionBucketDto> Distribution,
    IReadOnlyList<CsatAgentBucketDto> ByAgent);

public sealed record CsatDistributionBucketDto(int Rating, int Count);

public sealed record CsatAgentBucketDto(
    string? AgentId,
    string? AgentName,
    int Count,
    double AverageRating);
