namespace Crm.Identity.Api.Domain;

/// <summary>SDD CRM-035 / specs/004-identity-admin — permission catalog.</summary>
public static class PermissionCatalog
{
    public const string UsersManage = "users.manage";
    public const string RolesView = "roles.view";
    public const string TicketsWork = "tickets.work";
    public const string TicketsAssign = "tickets.assign";
    public const string TicketsAll = "tickets.*";
    public const string CustomersRead = "customers.read";
    public const string CustomersAll = "customers.*";

    public static readonly string[] All =
    [
        UsersManage,
        RolesView,
        TicketsWork,
        TicketsAssign,
        TicketsAll,
        CustomersRead,
        CustomersAll
    ];
}

public static class RoleNames
{
    public const string Admin = "Admin";
    public const string Lead = "Lead";
    public const string Agent = "Agent";
    public const string Customer = "Customer";

    public static readonly string[] All = [Admin, Lead, Agent, Customer];
}
