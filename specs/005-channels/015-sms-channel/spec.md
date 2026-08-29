# Feature Specification: SMS channel

**Story**: CRM-011, CRM-040 (SMS stub)  
**Epic**: Communication Channels / Integrations  
**Priority**: Should  
**Status**: Implemented  
**Created**: 2026-08-25

## User story

**As a** support agent,  
**I want** SMS conversations to become tickets with inbound/outbound messages on the thread,  
**so that** I can handle SMS demand in the same workspace as email, WhatsApp, and live chat.

## Business value

Completes the Should communication channels set so agents can work text messages without a separate SMS console.

## Scope

**In scope**
- CRM-011: ingest inbound SMS (dev/stub) → find-or-create customer by phone → create ticket → store `ChannelMessage` with `channel: Sms`
- CRM-040 stub: pluggable `SmsProvider` with **DevSmsProvider** (parse JSON inbound; log outbound)
- `POST /api/channels/intake/sms` and `POST /api/channels/tickets/:ticketId/messages/sms`
- Agent ticket detail: SMS reply option; thread shows Sms badges
- Automated Nest validation tests
- Demo curl in slice README

**Out of scope**
- Real Twilio/Nexmo/AWS SNS SDKs, delivery receipts, MMS/media
- Richer CRM-040 provider wiring beyond DevSmsProvider
- Portal SMS UI (API + agent reply is enough for UAT)
- Gateway route changes (catch-all already covers `/api/channels/**`)

## Preconditions

- Gateway + Channels + Customers + Tickets; agent at `http://localhost:5000`
- Demo: `agent@crm.local` / `Crm!123`

## Screens

| Screen | URL | Actor | Observable result |
|---|---|---|---|
| Agent ticket detail | agent ticket detail | agent | Sms messages + SMS reply channel |
| (API) SMS ingest | `POST /api/channels/intake/sms` | system/dev | Returns ticket number |

## Acceptance criteria

1. **Given** a valid SMS ingest payload (from phone, body), **when** posted through the gateway, **then** a ticket is created and an inbound Sms message is stored.
2. **Given** that ticket, **when** the agent sends an SMS reply, **then** an outbound Sms message appears on the thread.
3. **Given** the same phone twice, **when** I ingest again, **then** the customer is reused and a new ticket/message is created.
4. **Given** invalid payload (missing/invalid from or empty body), **when** I ingest or reply, **then** the API returns a clear validation error.
5. **Given** no resolvable phone on the ticket, **when** reply is posted, **then** API returns a clear error (not a crash).

## Definition of Done

- [x] AC pass on gateway UAT
- [x] Automated Nest validation tests cite CRM-011
- [x] README curl for demo ingest/reply
- [x] Spec/plan/code cite CRM-011 / CRM-040
- [x] Product Next updated

## Assumptions and dependencies

- Depends on: `005-channels-portal`, WhatsApp channel patterns (`013`)
- Assumptions: phone stored like WhatsApp (portal `email` / `fromEmail` fields hold E.164); Dev provider only
