# Tasks: Audit log detail

**Input**: `specs/001-identity/052-audit-detail`

## Phase 1: Spec

- [x] T001 Write spec.md / plan.md / tasks.md

## Phase 2: Identity API

- [x] T002 `AuditLogDetailDto` + directory lookup for actor/target
- [x] T003 `GET /api/identity/audit/{id}` endpoint + admin gate
- [x] T004 Integration tests (detail + 404)

## Phase 3: admin-mfe

- [x] T005 `audit-detail` page + route `/admin/audit/:id`
- [x] T006 List row link “View details”; store `loadDetail`

## Phase N: Ship

- [x] T007 Smoke gateway `/admin/audit/{id}`
- [x] T008 Update 000-product + identity feature.md
