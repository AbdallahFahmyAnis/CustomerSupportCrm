# Feature Specification: Portal feedback read-only

**Story**: CRM-030 (extend)  
**Epic**: Customer Portal (`specs/010-portal`)  
**Priority**: Should  
**Status**: Implemented  
**Created**: 2026-08-29

## User story

**As a** customer,  
**I want** to see my submitted CSAT rating when I return to the feedback page,  
**so that** I know my feedback was recorded and cannot accidentally submit twice.

## Scope

**In scope**
- `GET /api/tickets/feedback?ticketNumber=` — returns existing feedback or 404
- Portal `/portal/feedback?ticket=…` loads existing feedback and shows read-only summary (rating, comment, submitted date)
- Hide submit form when feedback already exists; show “View feedback” on track list when `hasFeedback` is true
- Channels portal track DTO exposes `hasFeedback` per ticket

**Out of scope**
- Editing or deleting submitted feedback
- CSAT aggregates (CRM-033)

## Screens

| Screen | URL | Actor | Result |
|---|---|---|---|
| Portal feedback (read-only) | `/portal/feedback?ticket=TKT-…` | customer | Read-only rating/comment/date |
| Track requests | `/portal/track` | customer | “View feedback” vs “Rate support” |

## Acceptance criteria

1. **Given** feedback exists for a ticket, **when** I open `/portal/feedback?ticket=…`, **then** I see read-only CSAT and no submit button.
2. **Given** no feedback, **when** I open the form, **then** I can submit as before (CRM-030).
3. **Given** feedback exists, **when** I GET by ticket number, **then** the API returns the stored row.
4. **Given** no feedback, **when** I GET by ticket number, **then** the API returns 404.

## Definition of Done

- [x] AC pass on gateway portal UI
- [x] Automated test CRM-030 for GET feedback
- [x] Spec/plan/tasks cite CRM-030 / slice 053
