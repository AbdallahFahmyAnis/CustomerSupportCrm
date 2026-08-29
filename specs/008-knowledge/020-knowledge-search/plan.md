# Implementation Plan: Knowledge base search

**Spec**: `specs/008-knowledge/020-knowledge-search/spec.md`  
**Story**: CRM-022  
**Workflow stage after this plan**: Apply â†’ Test â†’ Mock â†’ Done

## Summary

Add a ranked `GET /api/knowledge/search` endpoint on Knowledge, a dedicated knowledge-mfe search page, and an agent ticket-detail knowledge panel. Keep Sqlite/EF; rank in process (title hit > body hit).

## Technical Context

**Language/Version**: .NET 9 / Angular  
**Edge**: `http://localhost:5000`  
**Owning service**: Knowledge  
**Owning MFE**: knowledge-mfe (+ agent-mfe consumer)

## Constitution Check

- [x] Spec exists with screens + AC
- [x] One vertical slice (search capability)
- [x] No new public port
- [x] Story id in new types/endpoints
- [x] VSA + CQRS

## Code to apply *(mandatory)*

| Area | Path | Story |
|---|---|---|
| Contracts | `Crm.Contracts/Knowledge` search DTOs | CRM-022 |
| API | `Features/Articles/SearchKnowledge/**` | CRM-022 |
| Domain | ranking/snippet helpers | CRM-022 |
| knowledge-mfe | `features/articles/article-search/**` | CRM-022 |
| agent-mfe | ticket-detail knowledge panel | CRM-022 |
| Tests | `Crm.Knowledge.Api.Tests` | CRM-022 |

## Ranking

- Require non-empty `q` (400 if blank)
- Score: +100 title contains (case-insensitive), +10 body contains; prefer Published (+1)
- Snippet: ~120 chars around first body match (or title if body miss)
- Filters: `kind`, `status`, `publishedOnly`

## Endpoint

`GET /api/knowledge/search?q=&kind=&status=&publishedOnly=`
