# Implementation Plan: Email channel intake

**Spec**: `specs/008-email-channel/spec.md`  
**Story**: CRM-008 / CRM-040  
**Workflow stage after this plan**: Apply → Test → Mock → Done

## Summary

Add Channels email ingest mirroring web-form intake, behind a DevEmailProvider stub (CRM-040). Widen message channel type to include `Email`. Show channel messages on agent ticket detail via gateway.

## Technical Context

**Owning service**: Channels (+ agent-mfe display)  
**Edge**: `http://localhost:5000` only

## Constitution Check

- [x] Spec with screens + AC
- [x] One vertical slice
- [x] No new public port
- [x] Story ids in new types/routes

## Code to apply

| Area | Path | Story |
|---|---|---|
| Domain | `channels.models.ts` channel union | CRM-008 |
| Provider | `infrastructure/email/email-provider.ts`, `dev-email.provider.ts` | CRM-040 |
| Intake | `features/intake/ingest-email/{route,schema,handler,service}.ts` | CRM-008 |
| Module | `app.module.ts` | CRM-008 |
| FE | agent-mfe ticket-detail + tickets.api messages | CRM-008 |
| Test | channels test for email schema | CRM-008 |
| Docs | README curl | CRM-008 |

## Apply notes

1. Reuse `DownstreamClient` + `ChannelsStore.addRequest` with `channel: 'Email'`.
2. PortalRequest row still stored for store API consistency (email = from).
3. Agent-mfe: Feature-Based page loads `/api/channels/tickets/:id/messages`.
