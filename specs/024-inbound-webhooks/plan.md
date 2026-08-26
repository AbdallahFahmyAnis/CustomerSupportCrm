# Implementation Plan: Inbound channel webhooks

**Spec**: `specs/024-inbound-webhooks/spec.md`  
**Story**: CRM-040  
**Workflow stage after this plan**: Apply → Test → Mock → Done

## Summary

Add Twilio SMS/WhatsApp webhook endpoints with HMAC-SHA1 signature verification, mapping form posts into existing ingest CQRS commands. Dev bypass when auth token is empty.

## Technical Context

**Language/Version**: NestJS  
**Edge**: `http://localhost:5000`  
**Owning service**: Channels  
**Owning MFE**: none (ops/docs only)

## Constitution Check

- [x] Spec exists with screens + AC
- [x] One vertical slice (inbound webhook security)
- [x] No new public port
- [x] Story id in new types/endpoints
- [x] Feature folders + CQRS reuse

## Code to apply *(mandatory)*

| Area | Path | Story |
|---|---|---|
| Infra | `infrastructure/twilio/validate-twilio-signature.ts` | CRM-040 |
| Infra | `infrastructure/twilio/parse-twilio-inbound.ts` | CRM-040 |
| Config | `CHANNELS_PUBLIC_URL` in `app/config.ts` | CRM-040 |
| API | `features/webhooks/twilio-sms/**` | CRM-040 |
| API | `features/webhooks/twilio-whatsapp/**` | CRM-040 |
| DI | `app.module.ts` | CRM-040 |
| Tests | `test/twilio-signature.test.cjs` | CRM-040 |
| Docs | Channels README + product Next | CRM-040 |

## Endpoints

| Method | Path | Notes |
|---|---|---|
| POST | `/api/channels/webhooks/twilio/sms` | form-urlencoded; verify signature |
| POST | `/api/channels/webhooks/twilio/whatsapp` | form-urlencoded; strip `whatsapp:` |

Public URL for signing: `(CHANNELS_PUBLIC_URL \|\| http://localhost:5000)` + request path.
