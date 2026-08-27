namespace Crm.Tickets.Api.Domain;

/// <summary>SDD CRM-030 — post-resolution customer satisfaction feedback.</summary>
public sealed class TicketFeedback
{
    public Guid Id { get; private set; }
    public Guid TicketId { get; private set; }
    public int Rating { get; private set; }
    public string? Comment { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private TicketFeedback() { }

    public static TicketFeedback Create(Guid ticketId, int rating, string? comment)
    {
        if (rating is < 1 or > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5.");
        }

        return new TicketFeedback
        {
            Id = Guid.NewGuid(),
            TicketId = ticketId,
            Rating = rating,
            Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim(),
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public static TicketFeedback Rehydrate(
        Guid id, Guid ticketId, int rating, string? comment, DateTimeOffset createdAt) => new()
    {
        Id = id,
        TicketId = ticketId,
        Rating = rating,
        Comment = comment,
        CreatedAt = createdAt
    };
}
