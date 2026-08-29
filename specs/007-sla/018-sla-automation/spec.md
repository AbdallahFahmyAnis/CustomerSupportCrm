# Feature Specification: SLA auto-assign and escalation rules

**Story**: CRM-018, CRM-019  
**Epic**: SLA and Automation  
**Priority**: Should  
**Status**: Implemented  
**Created**: 2026-08-25

## User story

**As a** team lead,  
**I want** new tickets assigned automatically from rules and risky tickets escalated when SLA or priority rules say so,  
**so that** work reaches the right agent without manual triage every time.

## Business value

Cuts queue time on intake and surfaces breached or urgent work to a lead before customers wait.

## Scope

**In scope**
- CRM-018: SLA-owned **auto-assign rules** (category and/or priority â†’ agent); seeded defaults; admin list/edit
- CRM-018: On ticket create, Tickets asks SLA for a suggestion and assigns when still unassigned (fail open if SLA down)
- CRM-019: SLA **escalation settings** ”” escalate on first-response / resolution breach and/or always for Urgent; target agent (seed Lead)
- CRM-019: Tickets `run-automation` applies suggest-assign (if unassigned) + escalate when SLA says so
- Admin UI under `/admin/sla` for assign rules + escalation toggles
- Agent ticket detail: **Run automation** action
- Automated tests with story traits CRM-018 / CRM-019

**Out of scope**
- CRM-020 push/in-app notifications
- Round-robin / load-based assignment
- Background timer workers (on-demand automation only)
- Pausing SLA clocks for Waiting

## Preconditions

- `specs/007-sla/017-sla-targets` policies + evaluate
- Tickets assign / escalate APIs
- Demo users at `http://localhost:5000`

## Screens

| Screen | URL | Actor | Observable result |
|---|---|---|---|
| Admin SLA | `http://localhost:5000/admin/sla` | admin | Sections: policies (existing), auto-assign rules, escalation settings |
| Agent ticket detail | `http://localhost:5000/agent/tickets/{id}` | agent | **Run automation** updates assignee and/or escalated flag per rules |

## Acceptance criteria

1. **Given** seeded assign rules, **when** an unassigned ticket is created with matching category/priority, **then** it is assigned to the suggested agent.
2. **Given** SLA is unreachable, **when** a ticket is created, **then** create still succeeds and the ticket may remain unassigned.
3. **Given** escalation settings enable Urgent-always (or breach), **when** run-automation runs on a matching non-escalated ticket, **then** the ticket is escalated and optionally reassigned to the configured lead.
4. **Given** a ticket already escalated, **when** run-automation runs, **then** escalate is not applied again.
5. **Given** invalid rule payload (unknown priority / empty agent), **when** admin saves, **then** the API rejects without corrupting other rules.

## Definition of Done

- [x] AC pass on gateway UAT
- [x] Tests `[Trait("Story", "CRM-018")]` and `[Trait("Story", "CRM-019")]`
- [x] Seeded rules make admin + run-automation clickable
- [x] Spec/plan/code cite CRM-018 / CRM-019
- [x] Product Next updated

## Assumptions and dependencies

- Depends on: CRM-017, ticket lifecycle (003/010)
- Assumptions: agent ids match Tickets catalog (Demo / Lead); first-response milestone for breach uses “still New and unassigned” as unmet first response when timestamps are absent
