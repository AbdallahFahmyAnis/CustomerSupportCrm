# Implementation Plan: Portal feedback read-only

**Spec**: `specs/010-portal/053-feedback-read-only/spec.md`  
**Story**: CRM-030 (extend)

## Summary

Tickets API: `GetTicketFeedback` query by ticket number. Channels: `hasFeedback` on portal track rows. portal-mfe: load existing on init, read-only template branch, track list CTA swap.

## Code to apply

| Area | Path | Story |
|---|---|---|
| Tickets API | `Features/Tickets/GetTicketFeedback/*` | CRM-030 |
| Channels | `getTicketPortalMeta` / track DTO `hasFeedback` | CRM-030 |
| portal-mfe | `feedback.api.getByTicketNumber`, form read-only UI | CRM-030 |
| Tests | `TicketLifecycleTests` GET feedback | CRM-030 |
