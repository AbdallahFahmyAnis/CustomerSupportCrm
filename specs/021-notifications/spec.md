# Feature Specification: In-app alerts and notifications

**Story**: CRM-020  
**Epic**: SLA and Automation  
**Priority**: Should  
**Status**: Implemented  
**Created**: 2026-08-25

## User story

**As a** support agent,  
**I want** to see and dismiss in-app alerts in the CRM shell,  
**so that** I notice assignments and SLA risks without leaving my workspace.

## Business value

Keeps agents aware of important events in one place before email/SMS alerting is wired.

## Scope

**In scope**
- CRM-020: Nest **Notifications** service inbox (JSON file store) on `:5202`
- List notifications for the current user (`X-Crm-User-Id`), unread count, mark as read
- Seed demo alerts for Demo Agent
- Shell topbar bell + dropdown (badge, list, mark read)
- Automated Node tests citing CRM-020

**Out of scope**
- Email / SMS / push delivery
- WebSocket / SSE live push
- Producers from SLA/Tickets posting events (manual seed + optional create API for demos)
- CRM-016@ mentions

## Preconditions

- Gateway already proxies `/api/notifications`
- Demo: `agent@crm.local` / `Crm!123` at `http://localhost:5000`

## Screens

| Screen | URL | Actor | Observable result |
|---|---|---|---|
| Shell topbar | `http://localhost:5000/agent` (any signed-in page) | agent | Bell shows unread badge; opens list of seeded alerts; mark read clears badge |

## Acceptance criteria

1. **Given** a fresh notifications store, **when** the service starts, **then** Demo Agent has at least two unread notifications.
2. **Given** agent is signed in, **when** they open the bell, **then** they see their notifications (not another user’s).
3. **Given** an unread item, **when** they mark it read, **then** unread count decreases and the item shows as read.
4. **Given** missing user header, **when** list is called, **then** the API rejects (401/400).
5. **Given** gateway + notifications running, **when** `GET /health` runs, **then** aggregate includes `notifications` as ok.

## Definition of Done

- [x] AC pass on gateway UAT
- [x] Automated test citing CRM-020
- [x] Seed makes bell clickable
- [x] Spec/plan/code cite CRM-020
- [x] Product Next updated

## Assumptions and dependencies

- Depends on: gateway BFF identity headers; Identity demo users
- Assumptions: user id is `X-Crm-User-Id` GUID matching Identity
