# Feature Specification: Live chat channel

**Story**: CRM-010  
**Epic**: Communication Channels  
**Priority**: Should  
**Status**: Implemented  
**Created**: 2026-08-25

## User story

**As a** support agent,  
**I want** live chat conversations to become tickets with inbound/outbound messages on the thread,  
**so that** I can handle chat demand in the same workspace as email, WhatsApp, and portal.

## Business value

Adds the next Should channel after WhatsApp so agents can work real-time web chat without a separate inbox.

## Scope

**In scope**
- CRM-010: ingest inbound live chat (dev/stub) → find-or-create customer by email (or visitor session key) → create ticket → store `ChannelMessage` with `channel: LiveChat`
- Pluggable `ChatProvider` with **DevChatProvider** (accept JSON ingest; log outbound)
- `POST /api/channels/intake/chat` and `POST /api/channels/tickets/:ticketId/messages/chat`
- Agent ticket detail: Live chat reply form; thread shows LiveChat badges
- Portal: minimal chat widget page to start/continue a chat (demoable without curl)
- Automated Nest validation tests + README curl

**Out of scope**
- WebSockets / SignalR realtime push (poll or refresh is enough)
- CRM-011 SMS
- Third-party chat vendors (Intercom, Zendesk Chat, etc.)
- Typing indicators, presence, file sharing in chat
- Gateway route changes (catch-all already covers `/api/channels/**`)

## Preconditions

- Gateway + Channels + Customers + Tickets; agent/portal at `http://localhost:5000`
- Demo: `agent@crm.local` / `Crm!123`

## Screens

| Screen | URL | Actor | Observable result |
|---|---|---|---|
| Portal live chat | `http://localhost:5000/portal/chat` | customer | Can send a message and see ticket number |
| Agent ticket detail | agent ticket detail | agent | LiveChat messages + Live chat reply form |

## Acceptance criteria

1. **Given** a valid chat ingest payload (visitor email or session id + body), **when** posted through the gateway, **then** a ticket is created and an inbound LiveChat message is stored.
2. **Given** that ticket, **when** the agent sends a live chat reply, **then** an outbound LiveChat message appears on the thread.
3. **Given** the same visitor email twice, **when** I ingest again, **then** the customer is reused and a new ticket/message is created (or messages attach per API rules documented in plan).
4. **Given** invalid payload (missing body or missing identity), **when** I ingest or reply, **then** the API returns a clear validation error.
5. **Given** portal `/portal/chat`, **when** a visitor submits name, email, and message, **then** they see the ticket number and can send another message on the same ticket when the API supports continuation.

## Definition of Done

- [x] AC pass on gateway UAT
- [x] Automated Nest validation tests cite CRM-010
- [x] README curl for demo ingest/reply
- [x] Spec/plan/code cite CRM-010
- [x] Product Next updated

## Assumptions and dependencies

- Depends on: `005-channels-portal`, WhatsApp/email channel patterns (`008`/`009`/`013`)
- Assumptions: visitor identified primarily by email (same as portal web form); session id optional for display; Dev provider only; no realtime transport
