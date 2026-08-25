# Customers & Tickets EF Core (010 / CRM-037)

## Providers

| Mode | Config |
|------|--------|
| Sqlite (default / tests) | `Customers:DataPath` / `Tickets:DataPath` → `*-ef.db` |
| SQL Server | `ConnectionStrings:Customers` / `Tickets` + optional `*:Provider=SqlServer` |

```powershell
$env:Customers__Provider = "SqlServer"
$env:ConnectionStrings__Customers = "Server=localhost,1433;User Id=sa;Password=Crm_Local_Sql_2026!;TrustServerCertificate=True;Initial Catalog=CrmCustomers"
$env:Tickets__Provider = "SqlServer"
$env:ConnectionStrings__Tickets = "Server=localhost,1433;User Id=sa;Password=Crm_Local_Sql_2026!;TrustServerCertificate=True;Initial Catalog=CrmTickets"
```

Attachments remain under `{Customers:DataPath}/attachments`.
