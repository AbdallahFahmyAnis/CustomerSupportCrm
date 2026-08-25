# Feature Specification: WhatsApp channel

**Story**: CRM-009, CRM-040 (WhatsApp stub)  
**Epic**: Communication Channels / Integrations  
**Priority**: Should  
**Status**: Implemented  
**Created**: 2026-08-25

## User story

**As a** support agent,  
**I want** WhatsApp conversations to become tickets with inbound/outbound messages on the thread,  
**so that** I can handle WhatsApp demand in the same workspace as email and portal.

## Business value

Opens the next Must/Should channel after email without requiring a paid WhatsApp Business API for local demos.

## Scope

**In scope**
- CRM-009: ingest inbound WhatsApp (dev/stub) → find-or-create customer by phone → create ticket → store `ChannelMessage` with `channel: WhatsApp`
- CRM-040 stub: pluggable `WhatsAppProvider` with **DevWhatsAppProvider** (parse JSON inbound; log outbound)
- `POST /api/channels/intake/whatsapp` and `POST /api/channels/tickets/:ticketId/messages/whatsapp`
- Agent ticket detail: WhatsApp reply form; thread shows WhatsApp badges
- Automated Nest validation tests
- Demo curl in slice README

**Out of scope**
- Real Meta/Twilio WhatsApp SDKs, webhook signature verification, media/templates
- Live chat (CRM-010) / SMS (CRM-011)
- SendGrid email provider
- Gateway route changes (catch-all already covers `/api/channels/**`)

## Preconditions

- Gateway + Channels + Customers + Tickets; agent at `http://localhost:5000`
- Demo: `agent@crm.local` / `Crm!123`

## Screens

| Screen | URL | Actor | Observable result |
|---|---|---|---|
| Agent ticket detail | agent ticket detail | agent | WhatsApp messages + WhatsApp reply form |
| (API) WhatsApp ingest | `POST /api/channels/intake/whatsapp` | system/dev | Returns ticket number |

## Acceptance criteria

1. **Given** a valid WhatsApp ingest payload (from phone, body), **when** posted through the gateway, **then** a ticket is created and an inbound WhatsApp message is stored.
2. **Given** that ticket, **when** the agent sends a WhatsApp reply, **then** an outbound WhatsApp message appears on the thread.
3. **Given** the same phone twice, **when** I ingest again, **then** the customer is reused and a new ticket/message is created.
4. **Given** invalid payload (missing/invalid from or empty body), **when** I ingest or reply, **then** the API returns a clear validation error.
5. **Given** no resolvable phone on the ticket, **when** reply is posted, **then** API returns a clear error (not a crash).

## Definition of Done

- [x] AC pass on gateway UAT
- [x] Automated Nest validation tests cite CRM-009
- [x] README curl for demo ingest/reply
- [x] Spec/plan/code cite CRM-009 / CRM-040
- [x] Product Next updated

## Assumptions and dependencies

- Depends on: `005-channels-portal`, email channel patterns (`008`/`009-email-outbound`)
- Assumptions: phone stored in existing `fromEmail` / portal `email` fields as E.164 contact key; Dev provider only
