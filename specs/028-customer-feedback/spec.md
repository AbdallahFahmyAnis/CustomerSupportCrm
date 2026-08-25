# Feature Specification: Customer feedback (CSAT)

**Story**: CRM-030  
**Epic**: Customer Portal  
**Priority**: Should  
**Status**: Implemented  
**Created**: 2026-08-25

## User story

**As a** customer,  
**I want** to rate my resolved support experience and leave an optional comment,  
**so that** the team can improve service quality.

## Scope

**In scope**
- One CSAT feedback per ticket (rating 1–5 + optional comment) after Resolved/Closed
- Portal form `/portal/feedback` + home tile
- Agent ticket detail read-only CSAT when present
- Include feedback on `GET /api/tickets/{id}`

**Out of scope**
- Aggregates/dashboards (CRM-033); anonymous public reviews

## Screens

| Screen | URL | Actor | Result |
|---|---|---|---|
| Portal feedback | `/portal/feedback` | customer | Submit rating by ticket number |
| Ticket detail | `/agent/tickets/{id}` | agent | Read-only CSAT block when present |

## Acceptance criteria

1. Given a Resolved/Closed ticket, when I submit rating 1–5, then feedback is stored.
2. Given feedback already exists, when I submit again, then the API rejects the duplicate.
3. Given an open ticket, when I submit feedback, then the API rejects it.
4. Given feedback exists, when an agent opens the ticket, then they see the rating/comment.

## Definition of Done

- [x] AC + automated test CRM-030
- [x] Spec/plan/code cite CRM-030
