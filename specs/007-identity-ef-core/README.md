# OpenIddict + EF Core Identity + Channels TypeORM (007 / CRM-035 / CRM-037)

## Identity (.NET)

- **ASP.NET Core Identity** + **EF Core** (SQL Server when `ConnectionStrings:Identity` set; Sqlite otherwise)
- **OpenIddict** server: password + refresh at `POST /connect/token` (gateway BFF still uses `/api/identity/token*`)
- Demo users seeded via `UserManager` (`agent@crm.local` / `Crm!123`)

```powershell
docker compose up -d
$env:Identity__Provider = "SqlServer"
$env:ConnectionStrings__Identity = "Server=localhost,1433;User Id=sa;Password=Crm_Local_Sql_2026!;TrustServerCertificate=True;Initial Catalog=CrmIdentity"
```

## Channels (Nest)

- **TypeORM + PostgreSQL** when `CHANNELS_DATABASE_URL` is set
- JSON file fallback when unset (unit tests / no Docker)

```powershell
$env:CHANNELS_DATABASE_URL = "postgres://crm:Crm_Local_Pg_2026!@localhost:5432/crm_channels"
```
