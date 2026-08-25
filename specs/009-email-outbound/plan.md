# Implementation Plan: Outbound email reply

**Spec**: `specs/009-email-outbound/spec.md`  
**Story**: CRM-040  

## Summary

Add reply-email use case on Channels, extend EmailProvider with sendOutbound (Dev + optional SMTP), show reply UI on agent ticket detail.

## Code to apply

| Area | Path |
|---|---|
| Provider | `email-provider.ts`, `dev-email.provider.ts`, `smtp-email.provider.ts`, DI factory |
| Store | `addMessage`, `findRequestByTicketId` |
| Downstream | `getTicket`, `getCustomerEmail` |
| Feature | `features/messages/reply-email/{route,schema,handler,service,types}` |
| FE | tickets.api `replyEmail`, store + ticket-detail form |
| Test | validate reply body |
| Docs | README + product Next |

## Constitution Check

- [x] One slice, gateway-only, Channels owns messages
