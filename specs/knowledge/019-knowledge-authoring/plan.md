# Implementation Plan: Knowledge article authoring

**Spec**: `specs/knowledge/019-knowledge-authoring/spec.md`  
**Story**: CRM-021  
**Workflow stage after this plan**: Apply â†’ Test â†’ Mock â†’ Done

## Summary

Stand up `Crm.Knowledge.Api` on `:5104` with EF Core article CRUD, wire gateway `/api/knowledge`, and replace the knowledge-mfe stub with Feature-Based list/create/edit screens.

## Technical Context

**Language/Version**: .NET 9 / Angular  
**Edge**: `http://localhost:5000`  
**Owning service**: Knowledge (`:5104`)  
**Owning MFE**: knowledge-mfe

## Constitution Check

- [x] Spec exists with screens + AC
- [x] One vertical slice
- [x] No new public port
- [x] Story id in new types/endpoints
- [x] VSA + CQRS feature folders

## Code to apply *(mandatory)*

| Area | Path | Story |
|---|---|---|
| Contracts | `contracts/csharp/Crm.Contracts/Knowledge/*.cs` | CRM-021 |
| API | `src/services/knowledge/Crm.Knowledge.Api/**` | CRM-021 |
| Gateway | `appsettings.json` + health probe | CRM-021 |
| MFE | `knowledge-mfe/src/app/features/articles/**` | CRM-021 |
| Tests | `tests/Crm.Knowledge.Api.Tests` | CRM-021 |
| Product | `specs/000-product/spec.md` | |

## Model

**Article**: Id (Guid), Title, Body, Kind (`Faq|Article|Solution|Guide`), Status (`Draft|Published`), UpdatedAt, CreatedAt, CreatedBy.

## Endpoints

| Method | Path |
|---|---|
| GET | `/health` |
| GET | `/api/knowledge/articles?q=` |
| GET | `/api/knowledge/articles/{id}` |
| POST | `/api/knowledge/articles` |
| PUT | `/api/knowledge/articles/{id}` |

## Persistence

Sqlite under `data/knowledge-ef.db` by default; optional SQL Server via `ConnectionStrings:Knowledge`.
