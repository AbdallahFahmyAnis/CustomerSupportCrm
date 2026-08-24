namespace Crm.Customers.Api.Domain;

/// <summary>SDD CRM-003 / specs/002-customer-profiles — file attachment metadata.</summary>
public sealed class Attachment
{
    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long SizeBytes { get; private set; }
    public string StoragePath { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }

    private Attachment()
    {
    }

    public static Attachment Create(
        Guid customerId,
        string fileName,
        string contentType,
        long sizeBytes,
        string storagePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(storagePath);
        return new Attachment
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            FileName = Path.GetFileName(fileName),
            ContentType = string.IsNullOrWhiteSpace(contentType) ? "application/octet-stream" : contentType,
            SizeBytes = sizeBytes,
            StoragePath = storagePath,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public static Attachment Rehydrate(
        Guid id,
        Guid customerId,
        string fileName,
        string contentType,
        long sizeBytes,
        string storagePath,
        DateTimeOffset createdAt)
        => new()
        {
            Id = id,
            CustomerId = customerId,
            FileName = fileName,
            ContentType = contentType,
            SizeBytes = sizeBytes,
            StoragePath = storagePath,
            CreatedAt = createdAt
        };
}
