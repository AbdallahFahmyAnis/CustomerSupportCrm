# Implementation Plan: OpenIddict + Identity EF Core + Channels TypeORM

**Spec**: `specs/001-identity/007-identity-ef-core/spec.md`  
**Story**: CRM-035 / CRM-037  
**Workflow stage after this plan**: Apply â†’ Test â†’ Mock â†’ Done  
**Choice**: **A** ”” OpenIddict + ASP.NET Core Identity + EF Core

## Summary

Migrate Identity to ASP.NET Core Identity + OpenIddict backed by EF Core on SQL Server (SQLite/InMemory for offline tests). Keep existing `/api/identity/token*` and admin HTTP contracts so the gateway BFF is unchanged. Migrate Channels Nest store from JSON file to TypeORM on PostgreSQL. Amend constitution for EF Core (.NET) and TypeORM (Nest).

## Technical Context

**Language/Version**: .NET 9 / NestJS / Angular  
**Edge**: `http://localhost:5000`  
**Owning service**: Identity (+ Channels persistence)  
**Owning MFE**: none (API + infra)

## Constitution Check

- [x] Spec exists with screens + AC
- [x] One vertical slice (identity auth store + channels DB aligned to 006)
- [x] No new public port
- [x] Story ids in new types
- [x] Handlers stay in feature folders; persistence moves to Infrastructure EF/TypeORM

## Code to apply

| Area | Path | Story |
|---|---|---|
| Packages | `Directory.Packages.props`, Identity csproj | CRM-037 |
| EF | `Infrastructure/Persistence/IdentityAppDbContext.cs`, entities, migrations, seed | CRM-035 |
| OpenIddict | DI registration, token handlers using OpenIddict | CRM-035 |
| Remove/replace | `IdentityDb.cs` ADO; wire UserManager / OpenIddict in Auth + Users + Roles handlers | CRM-035 |
| Channels | TypeORM entities, data-source, replace `channels.store.ts` | CRM-037 |
| Compose/docs | connection strings in README 007; appsettings | CRM-037 |
| Tests | Identity factory â†’ EF Sqlite; Channels jest against Postgres or sqlite driver if feasible | CRM-035 |

## Apply notes

1. `ApplicationUser : IdentityUser<Guid>` with DisplayName; roles via IdentityRole; permissions as role claims (`Permission` claim type).
2. OpenIddict password + refresh flows; custom endpoints retain DTO shapes (`TokenResponseDto`, etc.).
3. Seed demo users with `UserManager` (Identity password hasher ”” not legacy SHA256).
4. SQL Server when `ConnectionStrings:Identity` set; else EF Sqlite file/dir for tests.
5. Channels: `pg` + TypeORM; `CHANNELS_DATABASE_URL` or discrete host/user/password; fallback JSON only if explicitly configured (prefer Postgres required when URL set).
6. Do not open Identity/Channels ports to browsers.
