# Implementation Plan: SLA response and resolution targets

**Spec**: `specs/017-sla-targets/spec.md`  
**Story**: CRM-017  
**Workflow stage after this plan**: Apply → Test → Mock → Done

## Summary

Stand up `Crm.Sla.Api` on `:5105` with EF Core Sqlite (SQL Server optional), seed priority policies, expose list/update + evaluate endpoints, proxy them through the gateway, and surface policies in admin-mfe plus due/breach on agent ticket detail.

## Technical Context

**Language/Version**: .NET 9 / Angular  
**Edge**: `http://localhost:5000`  
**Owning service**: SLA (`src/services/sla/Crm.Sla.Api`, port 5105)  
**Owning MFE**: admin-mfe (policies); agent-mfe (ticket SLA strip)

## Constitution Check

- [x] Spec exists with screens + AC
- [x] One vertical slice (SLA policies + evaluate; UI consumers only)
- [x] No new public port (5105 stays behind gateway)
- [x] Story id in new types/endpoints
- [x] Command/query live in a feature folder (VSA + CQRS)

## Code to apply *(mandatory)*

| Area | Path | Story |
|---|---|---|
| Contracts | `contracts/csharp/Crm.Contracts/Sla/*.cs` | CRM-017 |
| API | `src/services/sla/Crm.Sla.Api/**` (Features, Domain, Infrastructure) | CRM-017 |
| Gateway | `src/gateway/Crm.Gateway/appsettings.json`, `Program.cs` health probes | CRM-017 |
| Solution | `Crm.sln` + `tests/Crm.Sla.Api.Tests` | CRM-017 |
| Admin UI | `src/frontend/projects/admin-mfe/.../features/sla/**` + home tile + routes | CRM-017 |
| Agent UI | `src/frontend/projects/agent-mfe/.../ticket-detail/**` + tickets API evaluate call | CRM-017 |
| Product | `specs/000-product/spec.md` Next/Shipped | CRM-017 |
| Docs | `src/services/sla/README.md` | CRM-017 |

## Data model

**SlaPolicy** (priority key): `Priority` (TEXT PK), `FirstResponseMinutes` (int), `ResolutionMinutes` (int), `UpdatedAt` (DateTimeOffset).

Seed defaults (minutes): Low 480/2880, Medium 240/1440, High 60/480, Urgent 15/240.

## Endpoints (SLA service; gateway `/api/sla/...`)

| Method | Path | CQRS |
|---|---|---|
| GET | `/health` | Query |
| GET | `/api/sla/policies` | Query |
| PUT | `/api/sla/policies/{priority}` | Command |
| POST | `/api/sla/evaluate` | Query (body: priority, createdAt, optional firstResponseAt / resolvedAt / asOf) |

## Seed risks

- Sqlite Guid/string PK for priority is fine (catalog strings).
- `DateTimeOffset` ORDER BY not required for this slice.
- Seed only when empty; never fail startup.

## Out of this plan

Auto-assign, escalation rules, notifications, Tickets DB SLA columns.
