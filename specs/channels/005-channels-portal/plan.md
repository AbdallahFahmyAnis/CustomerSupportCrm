# Implementation Plan: Channels intake and customer portal

**Spec**: `specs/channels/005-channels-portal/spec.md`  
**Story**: CRM-012, CRM-027, CRM-028  
**Workflow stage after this plan**: Apply â†’ Test â†’ Mock â†’ Done

## Summary

Extend NestJS Channels with web-form intake that find-or-creates a Customer (email as unique id), creates a Ticket, and stores a portal request + inbound message. portal-mfe gets Feature-Based + Signals Submit and Track pages calling `/api/channels/...` via the gateway only.

## Technical Context

**Language/Version**: NestJS 10 + Angular 19  
**Edge**: `http://localhost:5000`  
**Owning service**: Channels (`src/services/channels`, :5201)  
**Owning MFE**: portal-mfe  
**Collaborators**: Customers (:5102), Tickets (:5103) over internal HTTP

## Constitution Check

- [x] Spec exists with screens + AC
- [x] One vertical slice (Channels + portal-mfe; Customers/Tickets called as collaborators)
- [x] No new public port
- [x] Story id in new types/endpoints
- [x] Command/query in feature folders (Nest CQRS + `features/{area}/{useCase}/`)

## API surface

| Method | Path | Purpose |
|---|---|---|
| POST | `/api/channels/intake/web-form` | CRM-012/027 submit |
| GET | `/api/channels/portal/requests?email=` | CRM-028 track |
| GET | `/api/channels/tickets/{ticketId}/messages` | inbound message for ticket (support smoke) |

Request body (submit): `{ name, email, subject, message }`  
Response: `{ ticketId, ticketNumber, requestId }`

## Persistence

JSON file under Channels data dir (e.g. `data/channels-store.json`): `requests[]`, `messages[]`. Seed one request for `portal.customer@example.com`.

## Frontend layout

```
src/frontend/projects/portal-mfe/src/app/features/requests/
  data-access/ requests.api.ts, requests.store.ts, request.models.ts
  pages/ portal-home.page.*, submit-request.page.*, track-requests.page.*
  requests.routes.ts
```

## Code to apply

| Area | Path | Story |
|---|---|---|
| Channels Nest | `src/services/channels/src/features/...` | CRM-012/027/028 |
| Channels store | `src/services/channels/src/persistence/` | CRM-012 |
| portal-mfe | `src/frontend/projects/portal-mfe/.../features/requests/` | CRM-027/028 |
| Tests | `src/services/channels` handler/unit or smoke script | CRM-012 |
| Mock | Channels seed | CRM-028 |

## Risks

- Customer create race: treat 409 duplicate as success and use `ExistingCustomerId`
- Tickets require Guid customerId ”” always resolve customer before create
- Do not call `:510x` / `:520x` from the browser
