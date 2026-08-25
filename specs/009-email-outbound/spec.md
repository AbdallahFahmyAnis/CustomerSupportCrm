# Feature Specification: Outbound email reply

**Story**: CRM-040 (email outbound); completes CRM-008 agent reply loop  
**Epic**: Integrations / Communication Channels  
**Priority**: Should  
**Status**: Implemented  
**Created**: 2026-08-25

## User story

**As a** support agent,  
**I want** to reply to a customer on the email channel from the ticket,  
**so that** the conversation stays on the ticket thread without leaving the CRM.

## Business value

Closes the email loop after inbound ingest; prepares a real SMTP/provider send path without locking to a paid SDK.

## Scope

**In scope**
- `POST /api/channels/tickets/:ticketId/messages/email` with `{ body }` (optional `to`)
- Resolve recipient from prior inbound Email/`PortalRequest` email, or Customers unique id/contact
- Persist `ChannelMessage` `channel: Email`, `direction: Outbound`
- Extend `EmailProvider` with `sendOutbound`; **DevEmailProvider** logs send; optional **SmtpEmailProvider** when `EMAIL_SMTP_URL` (or host/user/pass) is set
- Agent ticket detail: reply form under channel messages
- Validation tests for empty body

**Out of scope**
- SendGrid/Mailgun SDKs, signed webhooks
- SMS/WhatsApp providers
- Rich HTML / attachments
- Changing gateway public edge

## Preconditions

- Email ingest (008) or portal request exists for the ticket so a recipient email is known, **or** customer has email unique id
- Demo agent signed in at `http://localhost:5000`

## Screens

| Screen | URL | Actor | Observable result |
|---|---|---|---|
| Ticket detail | agent ticket detail | agent | Reply box; after send, outbound Email message appears in thread |

## Acceptance criteria

1. **Given** a ticket with a known customer email, **when** the agent posts a non-empty reply, **then** an outbound Email message is stored and listed on the ticket.
2. **Given** empty body, **when** reply is posted, **then** API returns validation error.
3. **Given** no recipient can be resolved, **when** reply is posted, **then** API returns a clear error (not a crash).
4. **Given** Dev provider, **when** reply succeeds, **then** send is acknowledged (logged) without requiring SMTP.
5. **Given** SMTP env is configured, **when** reply succeeds, **then** provider attempts SMTP send (best-effort; still store message if send fails only if we document — prefer fail the request if SMTP configured and send fails).

## Definition of Done

- [x] AC pass via gateway + agent UI
- [x] Automated validation test
- [x] Spec/plan/code cite CRM-040
- [x] README notes SMTP env vars

## Assumptions and dependencies

- Depends on: `008-email-channel`
- Assumptions: one recipient email per ticket for this slice; no CC/BCC
