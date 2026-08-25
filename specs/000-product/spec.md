# Product: Customer Support CRM

**Status**: Active  
**Created**: 2026-08-24

## North star

Give support agents a single place to know the customer, work tickets, and reply on the channel the customer used — in Arabic or English — while customers can submit and track requests in a portal.

## Personas

- **Support agent** — works assigned tickets with customer context
- **Team lead / manager** — assigns work, watches SLA and CSAT
- **Customer** — submits and tracks tickets, reads FAQs
- **Administrator** — users, roles, configuration
- **Knowledge author** — publishes reusable answers

## Shipped

- [x] `specs/001-platform-foundation` — gateway, bilingual shell, health, VSA example
- [x] `specs/002-customer-profiles` — CRM-001…003 profiles, contacts, notes/attachments
- [x] `specs/003-ticket-lifecycle` — CRM-004…007 tickets (Feature-Based + Signals FE)
- [x] `specs/004-identity-admin` — CRM-035 users, roles, permissions
- [x] `specs/005-channels-portal` — CRM-012 / CRM-027 / CRM-028 web form + portal submit/track
- [x] `specs/006-data-platform` — Docker SQL Server / Postgres / Mongo + Azure Pipelines; Identity SQL path
- [x] `specs/007-identity-ef-core` — OpenIddict + ASP.NET Identity + EF Core; Channels TypeORM/Postgres
- [x] `specs/008-email-channel` — CRM-008 email intake + CRM-040 DevEmailProvider stub
- [x] `specs/009-email-outbound` — CRM-040 outbound email reply (dev + optional SMTP)
- [x] `specs/010-customers-tickets-ef` — Customers/Tickets EF Core (SQL Server / Sqlite)
- [x] `specs/011-audit-logs` — CRM-036 Identity audit log API + admin-mfe list
- [x] `specs/012-system-config` — CRM-037 admin system settings UI + lockout policy
- [x] `specs/013-whatsapp-channel` — CRM-009 WhatsApp intake + reply (DevWhatsAppProvider)
- [x] `specs/014-live-chat` — CRM-010 live chat intake + reply (DevChatProvider) + portal chat
- [x] `specs/015-sms-channel` — CRM-011 SMS intake + reply (DevSmsProvider)
- [x] `specs/016-channel-providers` — CRM-040 SendGrid + Twilio SMS/WhatsApp adapters
- [x] `specs/017-sla-targets` — CRM-017 response/resolution SLA policies + ticket due/breach
- [x] `specs/018-sla-automation` — CRM-018/019 auto-assign rules + escalation automation
- [x] `specs/019-knowledge-authoring` — CRM-021 knowledge article authoring (FAQ/Article/Solution/Guide)
- [x] `specs/020-knowledge-search` — CRM-022 ranked knowledge search + agent panel
- [x] `specs/022-portal-faqs` — CRM-029 portal FAQ browse + detail

## Next

1. CRM-020 alerts/notifications when ready
2. CRM-016@ / richer collaboration when ready
3. Inbound webhooks / signature verification for providers

## Constraints

- Spec before code (`specs/WORKFLOW.md`)
- Gateway-only public edge (`http://localhost:5000`)
- .NET vertical slice + CQRS + DDD; NestJS for channels / AI / realtime
- Angular Native Federation; MFEs never call downstream ports
- Demo user: `agent@crm.local` / `Crm!123` (also `admin@crm.local` for admin)
