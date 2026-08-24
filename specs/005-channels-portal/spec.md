# Feature Specification: Channels intake and customer portal

**Story**: CRM-012, CRM-027, CRM-028  
**Epic**: Communication Channels + Customer Portal  
**Priority**: Must  
**Status**: Implemented  
**Created**: 2026-08-24

## User story

**As a** customer,  
**I want** to submit a support request from the portal (web form) and track my past requests by email,  
**so that** I can get help without calling an agent and still see what I already opened.

## Business value

Inbound demand is captured as tickets automatically; customers self-serve tracking instead of emailing “what is the status?”.

## Scope

**In scope**
- CRM-012: capture support requests from a web form (portal form → Channels intake)
- CRM-027: submit tickets from the customer portal
- CRM-028: track requests and view history by requester email
- Channels (NestJS) owns intake + portal request history messages; creates/links Customers + Tickets via service HTTP
- portal-mfe Feature-Based + Signals: submit + track screens under `/portal`
- Seed at least one demo portal request so Track is clickable

**Out of scope**
- CRM-008 email provider ingest, CRM-009 WhatsApp, CRM-010 live chat, CRM-011 SMS
- CRM-029 FAQs, CRM-030 feedback
- CRM-040 real email/SMS/WhatsApp provider SDKs
- Customer account login / SSO for portal (email lookup only for this slice)
- Agent UI to reply on channel threads (tickets still appear in agent queue)

## Preconditions

- `001-platform-foundation`, `002-customer-profiles`, `003-ticket-lifecycle` available
- Gateway proxies `/api/channels/*`; Customers `:5102`, Tickets `:5103`, Channels `:5201`
- Portal reachable at `http://localhost:5000/portal` without agent sign-in

## Screens

| Screen | URL | Actor | Observable result |
|---|---|---|---|
| Portal home | `http://localhost:5000/portal` | customer | Links to Submit and Track |
| Submit request | `http://localhost:5000/portal/submit` | customer | Form (name, email, subject, message); success shows ticket number |
| Track requests | `http://localhost:5000/portal/track` | customer | Enter email → list of prior requests with ticket number / subject / status |
| Agent tickets | `http://localhost:5000/agent/tickets` | agent | Portal-created tickets appear in the queue (smoke) |

## Acceptance criteria

1. **Given** I open Submit, **when** I enter name, email, subject, and message and submit, **then** I receive a confirmation with a ticket number.
2. **Given** a successful submit, **when** an agent opens the ticket queue, **then** the new ticket is listed for that customer.
3. **Given** I submitted with an email, **when** I open Track and search that email, **then** I see my request(s) with ticket number and subject.
4. **Given** an unknown email on Track, **when** I search, **then** I see an empty list (not an error crash).
5. **Given** invalid form input (missing email/subject), **when** I submit, **then** the API rejects with a clear validation error.
6. **Given** the gateway is up, **when** I call Channels intake via `/api/channels/...`, **then** traffic goes through the gateway only (no browser calls to `:5201`).

## Definition of Done

- [ ] AC pass on gateway UAT (manual: restart Customers/Tickets/Channels/Gateway + portal)
- [x] Automated coverage for intake validation citing CRM-012/027 (`npm test` in channels)
- [x] Seed makes Track clickable without manual setup (`portal.customer@example.com`)
- [x] Spec/plan/code cite CRM-012 / CRM-027 / CRM-028; portal-mfe Feature-Based + Signals with html/scss/ts

## Assumptions and dependencies

- Depends on: Customers create/search, Tickets create/get
- Assumptions: portal customers use email as `UniqueIdentifier`; Channels uses JSON file persistence for local demo; category/priority default to General / Medium
- Demo track email will be documented in seed (e.g. `portal.customer@example.com`)
