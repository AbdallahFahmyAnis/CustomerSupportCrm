# Data platform (006 / CRM-037)

## Local Docker databases

```bash
docker compose up -d
```

| Service | Port | Default credentials |
|---|---|---|
| SQL Server | 1433 | `sa` / `Crm_Local_Sql_2026!` |
| PostgreSQL | 5432 | `crm` / `Crm_Local_Pg_2026!` db `crm_channels` |
| MongoDB | 27017 | `crm` / `Crm_Local_Mongo_2026!` |

Copy `.env.example` → `.env` to override passwords.

### Run Identity on SQL Server

```powershell
$env:Identity__Provider = "SqlServer"
$env:ConnectionStrings__Identity = "Server=localhost,1433;User Id=sa;Password=Crm_Local_Sql_2026!;TrustServerCertificate=True;Initial Catalog=CrmIdentity"
```

Without those variables, Identity keeps using **SQLite** under `Identity:DataPath` (offline / unit-test default).

## Engine map

| Engine | Now | Later |
|---|---|---|
| SQL Server | Identity | Customers, Tickets |
| PostgreSQL | (compose only) | Channels |
| MongoDB | (compose only) | Knowledge |

## Azure Pipelines

`azure-pipelines.yml` starts the three DBs as service containers and runs .NET restore/build/test. Identity tests use SQL Server when `CRM_IDENTITY_PROVIDER=SqlServer` is set in the job.
