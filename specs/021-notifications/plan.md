# Implementation Plan: In-app alerts and notifications

**Spec**: `specs/021-notifications/spec.md`  
**Story**: CRM-020  
**Workflow stage after this plan**: Apply → Test → Mock → Done

## Summary

Extend Nest Notifications with a JSON inbox store, list/unread-count/mark-read APIs, seed for Demo Agent, and a shell topbar bell dropdown.

## Technical Context

**Language/Version**: NestJS / Angular  
**Edge**: `http://localhost:5000`  
**Owning service**: Notifications (`:5202`)  
**Owning MFE**: shell (chrome)

## Constitution Check

- [x] Spec with screens + AC
- [x] One vertical slice
- [x] No new public port
- [x] Story id in features
- [x] Nest feature folders + CQRS

## Code to apply *(mandatory)*

| Area | Path | Story |
|---|---|---|
| Store | `notifications/src/infrastructure/database/**` | CRM-020 |
| Features | `list-inbox`, `unread-count`, `mark-read` | CRM-020 |
| Shell | `main-layout` bell + notifications api | CRM-020 |
| Tests | `notifications/test/*.test.ts` | CRM-020 |
| Product | `specs/000-product/spec.md` | |

## Endpoints

| Method | Path |
|---|---|
| GET | `/api/notifications` |
| GET | `/api/notifications/unread-count` |
| POST | `/api/notifications/:id/read` |

## Seed

User `11111111-1111-1111-1111-111111111111`: assignment + SLA warning (unread).
