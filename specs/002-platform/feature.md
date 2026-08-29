# Feature: Platform

**Slug**: `002-platform`  
**Status**: Active (multi-slice)  
**Owning**: Gateway, Docker data, shared persistence  

## Stories

| Story | Slice | Status | Summary |
|---|---|---|---|
| CRM-041 / foundation | [001-platform-foundation](001-platform-foundation/spec.md) | Implemented | Gateway, shell, health, VSA |
| CRM-037 | [006-data-platform](006-data-platform/spec.md) | Implemented | Docker SQL / Postgres / Mongo + pipelines |
| — | [010-customers-tickets-ef](010-customers-tickets-ef/spec.md) | Implemented | Customers + Tickets EF Core |

## Related

- Identity / Channels consume engines from `006`
- Domain slices under `003-customers/` and `004-tickets/`
