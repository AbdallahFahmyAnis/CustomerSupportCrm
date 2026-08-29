# Implementation Plan: Cross-service audit + pagination

**Spec**: `specs/001-identity/051-audit-cross-service/spec.md`  
**Story**: CRM-036 (extend)

## Summary

Paginated audit list with `Service` column; internal ingest endpoint; fail-open writers from domain services; admin UI pager.

## Code to apply

| Area | Path | Story |
|---|---|---|
| Identity | paginated list + ingest endpoint | CRM-036 |
| Clients | Customers/Tickets/Knowledge/SLA audit writers | CRM-036 |
| admin-mfe | pager + Service column | CRM-036 |
| Tests | page envelope + ingest trait | CRM-036 |
