# Feature Specification: Audit log detail

**Story**: CRM-036 (extend)  
**Epic**: Identity & Access (`specs/001-identity`)  
**Priority**: Must  
**Status**: Implemented  
**Created**: 2026-08-29

## User story

**As an** administrator,  
**I want** to open a single audit event and see full actor, target, and metadata,  
**so that** I can investigate security incidents without guessing from the list row alone.

## Scope

**In scope**
- `GET /api/identity/audit/{id}` — Admin only; returns `AuditLogDetailDto` with actor/target display names, emails, and user ids when known
- admin-mfe **Audit detail** page at `/admin/audit/{id}` linked from the list (“View details”)
- Bilingual labels (EN/AR) for detail fields
- 404 when the id does not exist; 403 for non-admin

**Out of scope**
- Editing or deleting audit rows
- Diff view of before/after payloads
- Export / SIEM

## Screens

| Screen | URL | Actor | Observable result |
|---|---|---|---|
| Audit detail | `http://localhost:5000/admin/audit/{id}` | admin | Full event: action, service, success, timestamp, actor, target, detail |

## Acceptance criteria

1. **Given** an admin on the audit list, **when** I click View details, **then** I see the full event for that id.
2. **Given** a valid audit id, **when** admin GETs `/api/identity/audit/{id}`, **then** the response includes actor and target display fields when resolvable.
3. **Given** a random guid, **when** admin GETs detail, **then** the API returns 404.
4. **Given** a non-admin, **when** GET detail, **then** 403.

## Definition of Done

- [x] AC pass on gateway admin UI
- [x] Automated test `[Trait("Story", "CRM-036")]` for GET detail
- [x] Spec/plan/tasks cite CRM-036 / slice 052
- [x] Product backlog updated
