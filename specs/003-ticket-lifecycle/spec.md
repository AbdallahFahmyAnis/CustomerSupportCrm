# Feature Specification: Ticket lifecycle

**Story**: CRM-004, CRM-005, CRM-006, CRM-007  
**Epic**: Ticket Management  
**Priority**: Must  
**Status**: Implemented  
**Created**: 2026-08-24

## User story

**As a** support agent / team lead,  
**I want** to create, classify, assign, escalate, and progress tickets with a full history,  
**so that** every customer request has a durable, owned, auditable record.

## Business value

Work stops falling through the cracks; priority and ownership are visible in one queue.

## Scope

**In scope**
- CRM-004: create ticket linked to a customer; unique ticket id; search by id or customer
- CRM-005: set category and priority from configured lists; High/Urgent visually distinguished; required fields blocked
- CRM-006: assign / reassign / unassign; assignment recorded in history; assigned list filter
- CRM-007: status workflow with invalid-transition blocking; escalate flag; field-level history
- agent-mfe Feature-Based + Signals under `/agent/tickets`

**Out of scope**
- Bulk historical import
- AI auto-categorization (CRM-025)
- Automatic assignment / SLA escalation rules (CRM-018/019)
- Full RBAC denial paths (CRM-035) — any signed-in agent may assign in this slice

## Preconditions

- `001-platform-foundation` and `002-customer-profiles` available
- Seeded customers exist; agent signed in at `http://localhost:5000`

## Screens

| Screen | URL | Actor | Observable result |
|---|---|---|---|
| Ticket queue | `http://localhost:5000/agent/tickets` | agent | Searchable list; High/Urgent styled; Create |
| Create ticket | `http://localhost:5000/agent/tickets/new` | agent | Customer, subject, category, priority required |
| Ticket detail | `http://localhost:5000/agent/tickets/{id}` | agent | Status, assign, escalate, history |
| Assigned filter | `http://localhost:5000/agent/tickets?assignedTo=me` | agent | Only tickets assigned to current agent |

## Acceptance criteria

### CRM-004

1. **Given** required fields are provided, **when** I create a ticket, **then** it receives a unique ID and appears in the queue.
2. **Given** a ticket exists, **when** I search by ID or customer, **then** I can open the ticket.
3. **Given** a ticket is created, **when** I open it, **then** it is linked to the customer profile.

### CRM-005

4. **Given** a ticket, **when** I set category and priority from configured lists, **then** values are saved and visible in the queue.
5. **Given** priority is High or Urgent, **when** the ticket is saved, **then** it is visually distinguished in the queue.
6. **Given** category or priority is missing, **when** I try to save, **then** the system blocks save and explains what is missing.

### CRM-006

7. **Given** an unassigned ticket, **when** I assign it to an agent, **then** that agent sees it in their assigned list.
8. **Given** an assigned ticket, **when** I reassign it, **then** ownership changes and the assignment is recorded in history.
9. **Given** I unassign a ticket, **when** save completes, **then** it has no owner and history records the change.

### CRM-007

10. **Given** a ticket, **when** I move it to a valid next status, **then** the new status is saved.
11. **Given** a status transition is not allowed, **when** I attempt it, **then** the system blocks it and explains why.
12. **Given** I escalate a ticket, **when** escalation completes, **then** it is marked escalated (and may move to a lead queue/agent per config).
13. **Given** any field change, **when** I open ticket history, **then** I see who changed what and when.

## Definition of Done

- [x] AC pass on gateway UAT
- [x] Tests `[Trait("Story", "CRM-004")]` … `CRM-007`
- [x] Seeded tickets clickable without manual setup
- [x] Spec/plan/code cite story ids; FE uses Feature-Based + Signals

## Assumptions and dependencies

- Depends on: `002-customer-profiles` (customer id link)
- Assumptions: SQLite in Tickets service; customer display name denormalized at create time; agent directory is a small seeded list until Identity ships
- Constitution: FE Feature-Based + Signals (amended v1.1.0)
