# Feature Specification: Audit logs

**Story**: CRM-036  
**Epic**: Security and Administration  
**Priority**: Must  
**Status**: Implemented  
**Created**: 2026-08-25

## User story

**As an** administrator,  
**I want** to review audit logs of security-sensitive actions,  
**so that** I can see who signed in, changed roles, or deactivated users.

## Business value

Makes access control auditable (CRM-036 Must) without a separate SIEM for the local CRM.

## Scope

**In scope**
- Append-only audit entries in Identity (EF): actor, action, target, detail, timestamp, success
- Record: successful/failed login, create user, change role, deactivate user
- `GET /api/identity/audit?q=&take=` — Admin only
- admin-mfe **Audit** list page (Feature-Based + Signals)
- Seed a few demo audit rows on empty store
- Automated Identity test for admin list + forbidden for non-admin

**Out of scope**
- CRM-037 full system config UI
- Export/SIEM shipping, retention policies, immutability beyond append-only table
- Auditing every ticket/customer field change

## Screens

| Screen | URL | Actor | Observable result |
|---|---|---|---|
| Audit list | `http://localhost:5000/admin/audit` | admin | Filterable list of recent security events |

## Acceptance criteria

1. **Given** an admin session, **when** I open Audit, **then** I see recent events including seeded/demo activity.
2. **Given** a successful admin login and a user create, **when** I refresh Audit, **then** those actions appear.
3. **Given** a non-admin role header, **when** I GET audit API, **then** I receive 403.
4. **Given** a search query matching an action or email, **when** I filter, **then** the list narrows.

## Definition of Done

- [x] AC pass on gateway admin UI
- [x] Automated test `[Trait("Story", "CRM-036")]`
- [x] Spec/plan/code cite CRM-036
- [x] Product backlog updated

## Assumptions and dependencies

- Depends on: `004-identity-admin`, Identity EF Core
- Assumptions: gateway still forwards `X-Crm-User-Role` / `X-Crm-User-Id`
