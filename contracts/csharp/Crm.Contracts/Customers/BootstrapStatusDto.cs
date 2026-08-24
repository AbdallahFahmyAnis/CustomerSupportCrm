namespace Crm.Contracts.Customers;

/// <summary>SDD 001-platform-foundation / CRM-041 — Customers bootstrap query result.</summary>
public sealed record BootstrapStatusDto(
    string Service,
    string Status,
    string Slice,
    string Pattern);
