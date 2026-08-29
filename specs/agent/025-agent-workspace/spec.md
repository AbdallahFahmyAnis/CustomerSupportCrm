# Feature Specification: Agent workspace with customer context

**Story**: CRM-013  
**Epic**: Agent Dashboard  
**Priority**: Must  
**Status**: Implemented  
**Created**: 2026-08-25

## User story

**As a** support agent,  
**I want** a dashboard of my assigned tickets with the customer information beside the ticket,  
**so that** I can start work immediately without hunting for context.

## Business value

Agents open work already filtered to their queue and see customer contacts/notes without leaving the ticket.

## Scope

**In scope**
- CRM-013: Agent home “My tickets” strip (assigned to signed-in agent)
- Ticket list defaults to Assigned to me when session is present
- Ticket detail customer summary panel (profile + contacts + latest note via Customers API)
- Spec/plan/code cite CRM-013

**Out of scope**
- Workforce scheduling / shift planning
- Related tickets widget
- New Tickets API endpoints (reuse existing search + Customers GET)

## Preconditions

- CRM-004…007 tickets; CRM-001…003 customers; demo `agent@crm.local` / `Crm!123`

## Screens

| Screen | URL | Actor | Observable result |
|---|---|---|---|
| Agent home | `http://localhost:5000/agent` | agent | My tickets list with customer names |
| Ticket list | `http://localhost:5000/agent/tickets` | agent | Assigned to me on by default |
| Ticket detail | `http://localhost:5000/agent/tickets/{id}` | agent | Customer summary panel with contacts |

## Acceptance criteria

1. **Given** Demo Agent has assigned tickets, **when** they open agent home, **then** those tickets appear with customer name and priority/status.
2. **Given** a signed-in agent opens the ticket list, **when** the page loads, **then** Assigned to me is selected and the list is filtered.
3. **Given** a ticket detail is open, **when** the customer loads, **then** unique id, primary contacts, and latest note (if any) are visible without leaving the page.

## Definition of Done

- [x] AC pass on gateway UAT
- [x] Mock/seed makes screens clickable
- [x] Spec/plan/code cite CRM-013

## Assumptions and dependencies

- Depends on: CRM-001…007
- Session user id matches `TicketCatalog` agent ids for Demo Agent
