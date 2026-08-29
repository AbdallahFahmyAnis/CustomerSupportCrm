# Feature Specification: Richer channel providers

**Story**: CRM-040  
**Epic**: Integrations  
**Priority**: Should  
**Status**: Implemented  
**Created**: 2026-08-25

## User story

**As a** support operator,  
**I want** email, SMS, and WhatsApp sends to use real provider adapters when credentials are configured,  
**so that** demos can stay on Dev providers while UAT/staging can point at SendGrid and Twilio without changing ticket reply flows.

## Business value

Unblocks CRM-040 beyond SMTP/dev stubs so outbound channels share pluggable providers ready for production keys.

## Scope

**In scope**
- CRM-040: **SendGridEmailProvider** selected when `SENDGRID_API_KEY` is set (else SMTP if host set, else Dev)
- CRM-040: **TwilioSmsProvider** selected when Twilio account env is set (else DevSms)
- CRM-040: **TwilioWhatsAppProvider** selected when Twilio WhatsApp from-number env is set (else DevWhatsApp)
- HTTP-based adapters (no mandatory paid SDKs); fail the outbound reply if a configured provider send fails
- Channels README documents env vars; Nest validation/unit check for provider selection helpers
- Existing agent reply APIs unchanged

**Out of scope**
- Admin UI to edit provider secrets (stays env/config)
- Webhook signature verification for inbound SendGrid/Twilio/Meta
- Full Meta Cloud API OAuth app setup
- Changing gateway routes

## Preconditions

- Channels service with email/SMS/WhatsApp reply paths from 008”“015
- Demo still works with zero provider env (Dev* providers)

## Screens

| Screen | URL | Actor | Observable result |
|---|---|---|---|
| Agent ticket detail | agent ticket detail | agent | Same reply UX; outbound still appears on thread |
| (Ops) Channels README | `specs/005-channels/016-channel-providers/README.md` | admin | Env matrix for SendGrid / Twilio |

## Acceptance criteria

1. **Given** no provider env, **when** an agent replies on email/SMS/WhatsApp, **then** Dev providers log send and messages persist (existing behaviour).
2. **Given** `SENDGRID_API_KEY` is set, **when** email reply succeeds, **then** SendGrid HTTP send is used (fail request if SendGrid returns error).
3. **Given** Twilio SMS env is set, **when** SMS reply succeeds, **then** Twilio Messages API is used.
4. **Given** Twilio WhatsApp from env is set, **when** WhatsApp reply succeeds, **then** Twilio WhatsApp send is used.
5. **Given** invalid/missing body, **when** reply is posted, **then** validation still rejects as before.

## Definition of Done

- [x] AC pass with Dev providers (default)
- [x] Provider selection documented in README
- [x] Automated test for provider selection / SendGrid payload shape cite CRM-040
- [x] Spec/plan/code cite CRM-040
- [x] Product Next updated

## Assumptions and dependencies

- Depends on: `008`/`009` email, `013` WhatsApp, `015` SMS
- Assumptions: credentials only via env; inbound still Dev JSON parsers
