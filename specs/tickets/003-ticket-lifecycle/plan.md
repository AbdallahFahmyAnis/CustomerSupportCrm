# Implementation Plan: Ticket lifecycle

**Spec**: `specs/tickets/003-ticket-lifecycle/spec.md`  
**Story**: CRM-004 / CRM-005 / CRM-006 / CRM-007  
**Workflow stage after this plan**: Apply â†’ Test â†’ Mock â†’ Done

## Summary

Extend Tickets (.NET) with SQLite + VSA/CQRS for create, classify, assign, status, escalate, and history. agent-mfe gains a `features/tickets` module using Feature-Based + Signals (store + pages + presentational UI). Gateway already proxies `/api/tickets/*`.

## Technical Context

**Language/Version**: .NET 9 / Angular 19  
**Edge**: `http://localhost:5000`  
**Owning service**: Tickets (`Crm.Tickets.Api` :5103)  
**Owning MFE**: agent-mfe (`features/tickets`)

## Constitution Check

- [x] Spec exists with screens + AC
- [x] One vertical slice (Tickets + agent tickets feature)
- [x] No new public port
- [x] Story id in new types/endpoints
- [x] Command/query in feature folders
- [x] FE Feature-Based + Signals (constitution v1.1.0)

## Status workflow

`New` â†’ `InProgress` | `Waiting` | `Closed`  
`InProgress` â†’ `Waiting` | `Resolved` | `Closed`  
`Waiting` â†’ `InProgress` | `Resolved` | `Closed`  
`Resolved` â†’ `Closed` | `InProgress`  
`Closed` â†’ (none)

Categories: Billing, Technical, General. Priorities: Low, Medium, High, Urgent.

## Frontend layout

```
src/frontend/projects/agent-mfe/src/app/features/tickets/
  data-access/tickets.api.ts
  data-access/tickets.store.ts
  pages/ticket-list.page.ts
  pages/ticket-create.page.ts
  pages/ticket-detail.page.ts
  ui/ticket-priority-badge.component.ts
  tickets.routes.ts
```

## Code to apply

| Area | Path | Story |
|---|---|---|
| Domain/Infra | `src/services/tickets/Crm.Tickets.Api/` | CRM-004…007 |
| Contracts | `contracts/csharp/Crm.Contracts/Tickets/` | CRM-004…007 |
| Tests | `tests/Crm.Tickets.Api.Tests/` | CRM-004…007 |
| UI | `src/frontend/.../features/tickets/` | CRM-004…007 |
| Routes | agent `remote.routes.ts` load tickets children | CRM-004 |
| Nav | shell Customers-style Tickets link | CRM-004 |
