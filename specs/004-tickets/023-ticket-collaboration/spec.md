# Feature Specification: Ticket team collaboration

**Story**: CRM-016  
**Epic**: Agent Dashboard  
**Priority**: Should  
**Status**: Implemented  
**Created**: 2026-08-25

## User story

**As a** support agent,  
**I want** to mention colleagues and leave internal comments on a ticket,  
**so that** I can get help without exposing internal discussion to the customer.

## Business value

Agents get help in context of the ticket without leaking internal chatter to the portal customer.

## Scope

**In scope**
- CRM-016: Internal ticket notes (agent-only) on ticket detail
- @mentions of seeded catalog agents (by display name) when saving a note
- Mentioned agent receives an in-app notification (CRM-020 inbox) with link to the ticket
- Notes returned on ticket detail for agents; never exposed on portal request tracking
- Automated tests with `[Trait("Story", "CRM-016")]`

**Out of scope**
- Standalone team chat product
- Internal file attachments (defer)
- Real-time push / websockets
- Rich text editor
- Customer-visible conversation threads (channels already cover outbound)

## Preconditions

- `specs/004-tickets/003-ticket-lifecycle` ticket detail
- `specs/009-notifications/021-notifications` inbox + create producer path
- Demo: `agent@crm.local` / `Crm!123` at `http://localhost:5000`

## Screens

| Screen | URL | Actor | Observable result |
|---|---|---|---|
| Ticket detail | `http://localhost:5000/agent/tickets/{id}` | agent | Internal notes list + add note; @mention helpers |
| Shell alerts | `http://localhost:5000` topbar | mentioned agent | Unread mention notification opens the ticket |

## Acceptance criteria

1. **Given** a ticket, **when** an agent adds an internal comment, **then** it appears on ticket detail for agents.
2. **Given** the note body contains `@Lead Agent`, **when** the note is saved, **then** Lead Agent gets an unread in-app notification linking to that ticket.
3. **Given** a customer uses portal track-by-email, **when** they view request history, **then** internal notes are not shown.
4. **Given** an empty note body, **when** save is attempted, **then** the API rejects with a clear validation error.

## Definition of Done

- [x] Acceptance criteria pass in UAT on the gateway
- [x] Automated test `[Trait("Story", "CRM-016")]`
- [x] Mock/seed makes notes + mention clickable
- [x] Spec, plan, and code cite `CRM-016`

## Assumptions and dependencies

- Depends on: CRM-004…007, CRM-020
- Assumptions: mention matching uses `TicketCatalog` agent display names (`@Demo Agent`, `@Lead Agent`); notifications create API is service-to-service via gateway or direct URL
