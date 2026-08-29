# Tasks: Cross-service audit + pagination

**Input**: `specs/001-identity/051-audit-cross-service`

## Phase 1: Spec

- [x] T001 Write spec.md / plan.md / tasks.md

## Phase 2: Identity API

- [x] T002 Paginated list envelope `{ items, total, skip, take }`
- [x] T003 `Service` column; internal ingest `POST /api/identity/audit`
- [x] T004 Fail-open ingest clients in Customers/Tickets/Knowledge/SLA
- [x] T005 Integration tests CRM-036 pagination + ingest

## Phase 3: admin-mfe

- [x] T006 Pager UI; Service column; EN/AR

## Phase N: Ship

- [x] T007 Smoke cross-service events on `/admin/audit`
- [x] T008 Update 000-product + identity feature.md
