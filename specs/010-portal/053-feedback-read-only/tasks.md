# Tasks: Portal feedback read-only

**Input**: `specs/010-portal/053-feedback-read-only`

## Phase 1: Spec

- [x] T001 Write spec.md / plan.md / tasks.md

## Phase 2: Backend

- [x] T002 Tickets `GET /api/tickets/feedback?ticketNumber=`
- [x] T003 Channels track DTO `hasFeedback`
- [x] T004 Integration test GET feedback + 404

## Phase 3: portal-mfe

- [x] T005 `FeedbackApi.getByTicketNumber`; load on init / ticket change
- [x] T006 Read-only template; i18n keys (`viewFeedback`, `submittedOn`, …)
- [x] T007 Track list “View feedback” link

## Phase N: Ship

- [x] T008 Smoke gateway portal feedback for seeded ticket
- [x] T009 Update 000-product + portal feature.md
