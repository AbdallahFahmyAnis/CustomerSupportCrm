# Feature Specification: Inbound channel webhooks

**Story**: CRM-040  
**Epic**: Integrations  
**Priority**: Should  
**Status**: Implemented  
**Created**: 2026-08-25

## User story

**As a** support operator,  
**I want** Twilio SMS and WhatsApp inbound webhooks to reject forged requests when credentials are configured,  
**so that** tickets are only created from authentic provider callbacks.

## Business value

Closes the CRM-040 inbound gap so production-like Twilio webhooks can hit the gateway safely without leaving open JSON intake as the only path.

## Scope

**In scope**
- CRM-040: Twilio request signature verification (`X-Twilio-Signature`) for SMS and WhatsApp webhook endpoints
- Dev bypass when `TWILIO_AUTH_TOKEN` is unset (local demos keep working)
- Map Twilio form fields (`From`, `Body`) into existing SMS / WhatsApp ingest commands
- Keep existing JSON `/api/channels/intake/sms|whatsapp` for curl smoke
- Automated unit tests for signature helper cite CRM-040
- Document webhook URLs + `CHANNELS_PUBLIC_URL` in Channels README

**Out of scope**
- SendGrid Inbound Parse / Event webhooks
- Meta Cloud API signature verification
- Admin UI for webhook secrets
- Changing agent/portal screens

## Preconditions

- `specs/channels/016-channel-providers` outbound Twilio adapters
- Demo: `agent@crm.local` / `Crm!123` at `http://localhost:5000`

## Screens

| Screen | URL | Actor | Observable result |
|---|---|---|---|
| (Ops) Channels README | `src/services/channels/README.md` or `specs/channels/024-inbound-webhooks` | admin | Webhook paths + env documented |
| Agent ticket list | `http://localhost:5000/agent/tickets` | agent | Valid webhook creates/updates ticket thread |

## Acceptance criteria

1. **Given** `TWILIO_AUTH_TOKEN` is unset, **when** a Twilio-shaped form POST hits the SMS webhook, **then** ingest succeeds (dev bypass).
2. **Given** `TWILIO_AUTH_TOKEN` is set and signature is wrong/missing, **when** the webhook is called, **then** the API returns 401 and no ticket is created.
3. **Given** a valid signature, **when** Twilio SMS webhook posts `From`/`Body`, **then** the existing SMS ingest path runs and returns ticket ids.
4. **Given** a valid WhatsApp webhook (`From` may include `whatsapp:`), **when** signature is valid, **then** WhatsApp ingest runs.
5. **Given** existing JSON intake `/api/channels/intake/sms`, **when** called without signature, **then** behaviour is unchanged.

## Definition of Done

- [x] AC pass (unit + smoke)
- [x] Automated test cites CRM-040
- [x] README documents webhook URLs
- [x] Spec/plan/code cite CRM-040
- [x] Product Next updated

## Assumptions and dependencies

- Depends on: CRM-009, CRM-011, CRM-040 outbound
- Assumptions: Twilio signs the exact public URL (`CHANNELS_PUBLIC_URL` + webhook path); gateway already proxies `/api/channels/**`
