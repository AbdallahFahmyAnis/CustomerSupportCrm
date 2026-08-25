# Implementation Plan: Live chat channel

**Spec**: `specs/014-live-chat/spec.md`  
**Story**: CRM-010  
**Workflow stage after this plan**: Apply → Test → Mock → Done

## Summary

Mirror WhatsApp/email intake and reply for live chat: Nest features, DevChatProvider, widen channel union to `LiveChat`, agent reply form, and a minimal portal chat page for UAT.

## Technical Context

**Language/Version**: NestJS / Angular  
**Edge**: `http://localhost:5000`  
**Owning service**: Channels  
**Owning MFE**: agent-mfe + portal-mfe (thin chat widget)

## Constitution Check

- [x] Spec exists with screens + AC
- [x] One vertical slice (Channels + agent/portal UI)
- [x] No new public port
- [x] Story id in new types/endpoints
- [x] Command/query in feature folders

## Continuation rule

- First message: create ticket + inbound message (like WhatsApp ingest).
- Follow-up from portal: if `ticketId` is supplied and owned by that email, append inbound message only; otherwise create a new ticket.

## Code to apply *(mandatory)*

| Area | Path | Story |
|---|---|---|
| Domain | `channels.models.ts` add `LiveChat` | CRM-010 |
| Provider | `infrastructure/chat/*` | CRM-010 |
| Intake | `features/intake/ingest-chat/*` | CRM-010 |
| Reply | `features/messages/reply-chat/*` | CRM-010 |
| DI | `app.module.ts` | CRM-010 |
| FE agent | ticket detail LiveChat reply + badge | CRM-010 |
| FE portal | `/portal/chat` page | CRM-010 |
| Tests | Nest validation test | CRM-010 |
| Docs | README + product Next | CRM-010 |
