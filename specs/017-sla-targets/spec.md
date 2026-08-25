# Feature Specification: SLA response and resolution targets

**Story**: CRM-017  
**Epic**: SLA and Automation  
**Priority**: Should  
**Status**: Implemented  
**Created**: 2026-08-25

## User story

**As a** team lead / administrator,  
**I want** to define first-response and resolution SLA targets by ticket priority,  
**so that** agents and leads can see due times on a ticket and know when work is at risk.

## Business value

Makes support commitments measurable before automation (auto-assign / escalation) depends on those targets.

## Scope

**In scope**
- CRM-017: New **SLA** .NET service (`:5105`) owning priority-based policies (first-response minutes, resolution minutes)
- Seeded default policies for Low / Medium / High / Urgent
- Admin can list and update policy targets
- Evaluate due times / breach flags for a ticket’s priority + timestamps (no broker)
- Agent ticket detail shows response and resolution due (and breached state)
- Gateway proxy `/api/sla/...` + health probe

**Out of scope**
- CRM-018 automatic assignment
- CRM-019 rule-driven escalation
- CRM-020 alert notifications
- CRM-032 SLA performance dashboards
- Pausing SLA clocks for Waiting status (future)
- Persisting timers as ticket rows in Tickets DB (evaluation is on-demand from SLA)

## Preconditions

- Tickets lifecycle with Priority catalog (003 / 010)
- Admin MFE and agent ticket detail exist
- Seeded demo: `admin@crm.local` / `Crm!123` and `agent@crm.local` / `Crm!123` at `http://localhost:5000`

## Screens

| Screen | URL | Actor | Observable result |
|---|---|---|---|
| Admin SLA policies | `http://localhost:5000/admin/sla` | admin / lead | Table of priorities with editable response/resolution minutes; save updates targets |
| Agent ticket detail | `http://localhost:5000/agent/tickets/{id}` | agent | SLA strip: first-response due, resolution due; breached styling when overdue |

## Acceptance criteria

1. **Given** a fresh SLA database, **when** the service starts, **then** four priority policies are seeded (Low, Medium, High, Urgent) with non-zero targets.
2. **Given** an admin is signed in, **when** they open Admin → SLA and change High response minutes, **then** GET policies reflects the new value.
3. **Given** a ticket with priority High and a known `createdAt`, **when** evaluate is called (or ticket detail loads), **then** first-response and resolution due timestamps equal `createdAt` + policy minutes.
4. **Given** `now` is past a due timestamp and the milestone is unmet, **when** evaluate runs, **then** the corresponding breached flag is true.
5. **Given** gateway is up and SLA is running, **when** `GET /health` on the gateway runs, **then** the aggregate includes `sla` as ok.
6. **Given** invalid priority or non-positive minutes on update, **when** the API is called, **then** the request is rejected without corrupting other policies.

## Definition of Done

- [x] Acceptance criteria pass in UAT on the gateway
- [x] Automated test `[Trait("Story", "CRM-017")]` for policy seed/update and evaluate maths
- [x] Mock/seed data makes admin SLA and ticket SLA strip clickable
- [x] Spec, plan, and code cite `CRM-017`
- [x] Product Next updated

## Assumptions and dependencies

- Depends on: `003-ticket-lifecycle` / `010-customers-tickets-ef` (priority catalog), `004-identity-admin` (admin chrome)
- Assumptions: business hours are 24×7 for this slice; Waiting does not pause clocks yet
