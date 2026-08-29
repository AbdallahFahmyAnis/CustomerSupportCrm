# Feature Specification: Cross-service audit + pagination

**Story**: CRM-036 (extend)  
**Epic**: Identity & Access (`specs/identity`)  
**Priority**: Must  
**Status**: Implemented  
**Created**: 2026-08-29

## User story

**As an** administrator,  
**I want** a paged audit log that includes security-relevant actions from Identity and other CRM services,  
**so that** I can review who changed users, customers, tickets, knowledge, and SLA settings without loading everything at once.

## Scope

**In scope**
- Paginated `GET /api/identity/audit?q=&skip=&take=` → `{ items, total, skip, take }`
- `Service` on audit rows (Identity, Customers, Tickets, Knowledge, Sla, Channels)
- Internal ingest `POST /api/identity/audit` for service writers (fail-open clients)
- Record key mutations: Customers create/update; Tickets create/status; Knowledge publish/save; SLA policy/rules/escalation
- Admin audit UI: page size, prev/next, total, Service column; EN/AR

**Out of scope**
- Full field-level ticket history (stays on ticket)
- Export / SIEM / retention

## Acceptance criteria

1. Given admin, when I open Audit, then I see a page of events with total count and can go to the next page.
2. Given create customer / ticket / SLA save, when I refresh Audit, then a row appears with the owning service.
3. Given non-admin, when I GET audit, then 403.
4. Given ingest from a service, when Identity is down, then the business mutation still succeeds (fail-open).

## Definition of Done

- [x] AC on gateway UAT (restart Identity + dependent APIs; open `/admin/audit`)
- [x] Test Trait CRM-036 for page envelope + ingest
- [x] Spec/plan/code cite CRM-036 / 051
