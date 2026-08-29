# Implementation Plan: Multi-database Docker platform

**Spec**: `specs/platform/006-data-platform/spec.md`  
**Story**: CRM-037  
**Workflow stage after this plan**: Apply â†’ Test â†’ Mock â†’ Done

## Summary

Add Docker Compose for SQL Server, PostgreSQL, and MongoDB; add Azure Pipelines with matching service containers; switch Identity to SQL Server when a connection string is configured while keeping SQLite as the offline test escape hatch. Amend the constitution and document the engine map for later service migrations.

## Technical Context

**Language/Version**: .NET 9 / NestJS / Angular  
**Edge**: `http://localhost:5000` (unchanged)  
**Owning service**: Platform + Identity (proof)  
**Owning MFE**: none (infra)

## Constitution Check

- [x] Spec exists with screens + AC
- [x] One vertical slice (platform persistence + Identity proof)
- [x] No new public port
- [x] Story id in new infra docs / Identity SQL path
- [x] No new CQRS feature required (persistence infrastructure only)
- [x] Constitution amended to allow named Docker DBs

## Engine map (this + later)

| Engine | Compose service | First consumer |
|---|---|---|
| SQL Server | `sqlserver` :1433 | Identity (this slice); Customers/Tickets later |
| PostgreSQL | `postgres` :5432 | Channels later |
| MongoDB | `mongo` :27017 | Knowledge later (container only now) |

## Code to apply *(mandatory)*

| Area | Path | Story |
|---|---|---|
| Compose | `docker-compose.yml`, `.env.example` | CRM-037 |
| Pipeline | `azure-pipelines.yml` | CRM-037 |
| Identity SQL | `Crm.Identity.Api/Infrastructure/IdentityDb.cs`, `appsettings*.json`, csproj, `Directory.Packages.props` | CRM-037 |
| Tests | `tests/Crm.Identity.Api.Tests` ”” keep Sqlite default; honor `ConnectionStrings__Identity` when set | CRM-037 |
| Docs | `specs/platform/006-data-platform/*`, constitution, product Next | CRM-037 |

## Apply notes

1. SQL Server image: `mcr.microsoft.com/mssql/server:2022-latest`, SA password via env (`CRM_MSSQL_SA_PASSWORD`, strong default for local).
2. Identity: if `ConnectionStrings:Identity` present â†’ `Microsoft.Data.SqlClient`; else existing Sqlite `Identity:DataPath`.
3. SQL dialect: `@params`, `TOP`, `MERGE`/`IF NOT EXISTS` instead of Sqlite `ON CONFLICT` / `INSERT OR REPLACE` / `LIMIT`.
4. Pipeline: `DotNetCoreCLI` restore/build/test; service containers for three DBs; set Identity connection string for optional SQL-backed smoke if feasible; always keep Sqlite unit tests green without requiring Docker on every laptop.
5. Do not commit `*.db`, secrets beyond documented local defaults.
