# Feature Specification: Email channel intake

**Story**: CRM-008, CRM-040 (stub)  
**Epic**: Communication Channels / Integrations  
**Priority**: Must  
**Status**: Implemented  
**Created**: 2026-08-24

## User story

**As a** support agent,  
**I want** customer emails to become tickets with an inbound email message on the thread,  
**so that** I can work email demand in the same workspace as portal and other channels.

## Business value

Email is a Must channel; capturing it as tickets avoids a separate mailbox workflow and prepares real provider wiring (CRM-040).

## Scope

**In scope**
- CRM-008: ingest inbound email (dev/stub) → find-or-create customer → create ticket → store `ChannelMessage` with `channel: Email`
- CRM-040 stub: pluggable `EmailProvider` interface with a **DevEmailProvider** that accepts a simple JSON payload (no real SMTP/IMAP/SendGrid SDK)
- API: `POST /api/channels/intake/email` via gateway
- Agent ticket detail: show channel messages (including Email) from existing messages API
- Automated Nest test for email payload validation

**Out of scope**
- Real provider SDKs, webhooks signatures, MIME parsing, attachments
- Outbound agent email reply / SMTP send
- WhatsApp / SMS / live chat (CRM-009…011)
- Customer mailbox UI

## Preconditions

- Gateway + Channels + Customers + Tickets running; browser uses `http://localhost:5000`
- Demo agent: `agent@crm.local` / `Crm!123`

## Screens

| Screen | URL | Actor | Observable result |
|---|---|---|---|
| Agent ticket detail | `http://localhost:5000/agent/...` ticket detail | agent | Channel messages list includes inbound Email body/from |
| (API) Email ingest | gateway `POST /api/channels/intake/email` | system/dev | Returns ticket number; message stored |

## Acceptance criteria

1. **Given** a valid email ingest payload (from, subject, body), **when** posted through the gateway, **then** a ticket is created and a confirmation with ticket number is returned.
2. **Given** that ticket id, **when** I GET channel messages, **then** I see an inbound message with channel `Email`.
3. **Given** the same from-email twice, **when** I ingest again, **then** customer is reused (find-or-create) and a new ticket/message is created.
4. **Given** invalid payload (missing from/subject/body), **when** I ingest, **then** the API returns a clear validation error.
5. **Given** an agent opens ticket detail after ingest, **when** the page loads, **then** the Email message is visible without calling `localhost:520x` from the browser.

## Definition of Done

- [x] Acceptance criteria pass on gateway UAT
- [x] Automated test for email validation
- [x] Documented curl for demo ingest (`README.md`)
- [x] Spec, plan, and code cite CRM-008 / CRM-040

## Assumptions and dependencies

- Depends on: `005-channels-portal`, Tickets create, Customers find-or-create
- Assumptions: Dev provider only; no auth beyond existing gateway for demo ingest in local/dev
