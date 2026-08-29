# Feature Specification: Quick replies

**Story**: CRM-015  
**Epic**: Agent Dashboard  
**Priority**: Should  
**Status**: Implemented  
**Created**: 2026-08-25

## User story

**As a** support agent,  
**I want** a library of saved quick replies I can insert into a response,  
**so that** I answer common questions faster and consistently.

## Scope

**In scope**
- Shared catalog of seeded quick replies
- `GET /api/tickets/quick-replies`
- Agent ticket detail picker that inserts into email/chat draft (no new send API)

**Out of scope**
- Personal CRUD UI; AI suggestions (CRM-024)

## Screens

| Screen | URL | Actor | Result |
|---|---|---|---|
| Ticket detail | `/agent/tickets/{id}` | agent | Quick reply picker fills draft |

## Acceptance criteria

1. Given the catalog is seeded, when I open ticket detail, then I can pick a quick reply.
2. Given I pick a reply, when I insert it, then the email or chat draft contains that body.
3. Given I call the list API, then at least three shared replies are returned.

## Definition of Done

- [x] AC + automated test CRM-015
- [x] Spec/plan/code cite CRM-015
