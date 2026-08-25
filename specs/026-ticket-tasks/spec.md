# Feature Specification: Ticket tasks and reminders

**Story**: CRM-014  
**Epic**: Agent Dashboard  
**Priority**: Should  
**Status**: Implemented  
**Created**: 2026-08-25

## User story

**As a** support agent,  
**I want** to create tasks and reminders on tickets or for myself,  
**so that** follow-ups are not forgotten.

## Scope

**In scope**
- Tasks linked to tickets with title, optional due date, complete/cancel
- List tasks on ticket detail; list my due/open tasks for home
- Seed sample tasks; tests `[Trait("Story", "CRM-014")]`

**Out of scope**
- Personal calendar sync; notification push for due tasks

## Screens

| Screen | URL | Actor | Result |
|---|---|---|---|
| Ticket detail | `/agent/tickets/{id}` | agent | Tasks panel add/complete/cancel |
| Agent home | `/agent` | agent | Due/open tasks strip |

## Acceptance criteria

1. Given a ticket, when I add a task with a due date, then it appears on the ticket.
2. Given an open task, when I complete it, then it is marked completed and no longer open.
3. Given I cancel a task, then it is not listed as open.
4. Given tasks due today assigned to me, when I open agent home, then they appear under due tasks.

## Definition of Done

- [x] AC + automated test CRM-014
- [x] Spec/plan/code cite CRM-014
