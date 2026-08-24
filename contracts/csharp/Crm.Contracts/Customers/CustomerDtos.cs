namespace Crm.Contracts.Customers;

public sealed record CustomerSummaryDto(
    string Id,
    string DisplayName,
    string? Organization,
    string Status,
    string UniqueIdentifier);

public sealed record CustomerDetailDto(
    string Id,
    string DisplayName,
    string? Organization,
    string Status,
    string UniqueIdentifier,
    IReadOnlyList<ContactDto> Contacts,
    IReadOnlyList<NoteDto> Notes,
    IReadOnlyList<AttachmentDto> Attachments,
    IReadOnlyList<TimelineItemDto> Timeline);

public sealed record ContactDto(
    string Id,
    string Type,
    string Value,
    bool IsPrimary,
    bool IsActive);

public sealed record NoteDto(
    string Id,
    string Body,
    string AuthorName,
    DateTimeOffset CreatedAt);

public sealed record AttachmentDto(
    string Id,
    string FileName,
    string ContentType,
    long SizeBytes,
    DateTimeOffset CreatedAt);

public sealed record TimelineItemDto(
    string Id,
    string Kind,
    string Summary,
    DateTimeOffset OccurredAt);

public sealed record CreateCustomerRequest(
    string DisplayName,
    string UniqueIdentifier,
    string? Organization,
    string? Status);

public sealed record UpdateCustomerRequest(
    string DisplayName,
    string UniqueIdentifier,
    string? Organization,
    string? Status);

public sealed record AddContactRequest(
    string Type,
    string Value,
    bool IsPrimary);

public sealed record AddNoteRequest(string Body);

public sealed record DuplicateWarningDto(string Message, string ExistingCustomerId);
