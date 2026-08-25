namespace Crm.Sla.Api.Domain;

/// <summary>SDD CRM-017 — response/resolution targets for one priority.</summary>
public sealed class SlaPolicy
{
    public string Priority { get; private set; } = "";
    public int FirstResponseMinutes { get; private set; }
    public int ResolutionMinutes { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private SlaPolicy()
    {
    }

    public static SlaPolicy Create(string priority, int firstResponseMinutes, int resolutionMinutes)
    {
        if (!SlaCatalog.IsKnownPriority(priority))
        {
            throw new ArgumentException("Unknown priority.", nameof(priority));
        }

        ValidateMinutes(firstResponseMinutes, resolutionMinutes);
        return new SlaPolicy
        {
            Priority = SlaCatalog.NormalizePriority(priority),
            FirstResponseMinutes = firstResponseMinutes,
            ResolutionMinutes = resolutionMinutes,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }

    public static SlaPolicy Rehydrate(
        string priority,
        int firstResponseMinutes,
        int resolutionMinutes,
        DateTimeOffset updatedAt)
    {
        var policy = Create(priority, firstResponseMinutes, resolutionMinutes);
        policy.UpdatedAt = updatedAt;
        return policy;
    }

    public void Update(int firstResponseMinutes, int resolutionMinutes)
    {
        ValidateMinutes(firstResponseMinutes, resolutionMinutes);
        FirstResponseMinutes = firstResponseMinutes;
        ResolutionMinutes = resolutionMinutes;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private static void ValidateMinutes(int firstResponseMinutes, int resolutionMinutes)
    {
        if (firstResponseMinutes <= 0)
        {
            throw new ArgumentException("First response minutes must be positive.");
        }

        if (resolutionMinutes <= 0)
        {
            throw new ArgumentException("Resolution minutes must be positive.");
        }

        if (resolutionMinutes < firstResponseMinutes)
        {
            throw new ArgumentException("Resolution minutes must be >= first response minutes.");
        }
    }
}
