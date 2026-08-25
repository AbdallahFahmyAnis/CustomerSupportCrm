# Implementation Plan: Ticket team collaboration

**Spec**: `specs/023-ticket-collaboration/spec.md`  
**Story**: CRM-016  
**Workflow stage after this plan**: Apply → Test → Mock → Done

## Summary

Add internal ticket notes with @mention parsing on Tickets, a Notifications create producer for mention alerts, and an agent-mfe notes panel on ticket detail.

## Technical Context

**Language/Version**: .NET 9 / NestJS / Angular  
**Edge**: `http://localhost:5000`  
**Owning service**: Tickets (notes) + Notifications (mention alerts)  
**Owning MFE**: agent-mfe

## Constitution Check

- [x] Spec exists with screens + AC
- [x] One vertical slice (collaboration on a ticket)
- [x] No new public port
- [x] Story id in new types/endpoints
- [x] VSA + CQRS

## Code to apply *(mandatory)*

| Area | Path | Story |
|---|---|---|
| Contracts | `Crm.Contracts/Tickets` note DTOs | CRM-016 |
| Domain | `TicketNote`, mention parser | CRM-016 |
| API | `Features/Tickets/AddTicketNote/**` | CRM-016 |
| Infra | EF `TicketNotes` + GetTicket include | CRM-016 |
| Notifications | `create` store + `POST /api/notifications` | CRM-016 |
| Tickets client | HTTP notify on mention | CRM-016 |
| agent-mfe | ticket-detail notes panel | CRM-016 |
| Tests | `Crm.Tickets.Api.Tests` (+ Nest store test) | CRM-016 |

## Behaviour

- `POST /api/tickets/{id}/notes` body `{ "body": "..." }` — author from `X-Crm-User-*`
- Parse `@Display Name` against `TicketCatalog.Agents` (longest-name first)
- Persist note; `GetTicket` returns `notes` newest-first
- For each mentioned userId ≠ author: `POST` notification `{ userId, title, body, kind: "mention", href: "/agent/tickets/{id}" }`
- Failures to notify must not roll back the saved note (log / best-effort)

## Out of band

Portal `/api/channels/portal/requests` must not gain notes fields.
