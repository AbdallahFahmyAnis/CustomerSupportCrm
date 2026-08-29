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

- [x] `specs/002-platform/001-platform-foundation` — gateway, bilingual shell, health, VSA example
- [x] `specs/003-customers/002-customer-profiles` — CRM-001…003 profiles, contacts, notes/attachments
- [x] `specs/004-tickets/003-ticket-lifecycle` — CRM-004…007 tickets (Feature-Based + Signals FE)
- [x] `specs/001-identity/004-identity-admin` — CRM-035 users, roles, permissions
- [x] `specs/005-channels/005-channels-portal` — CRM-012 / CRM-027 / CRM-028 web form + portal submit/track
- [x] `specs/002-platform/006-data-platform` — Docker SQL Server / Postgres / Mongo + Azure Pipelines; Identity SQL path
- [x] `specs/001-identity/007-identity-ef-core` — OpenIddict + ASP.NET Identity + EF Core; Channels TypeORM/Postgres
- [x] `specs/005-channels/008-email-channel` — CRM-008 email intake + CRM-040 DevEmailProvider stub
- [x] `specs/005-channels/009-email-outbound` — CRM-040 outbound email reply (dev + optional SMTP)
- [x] `specs/002-platform/010-customers-tickets-ef` — Customers/Tickets EF Core (SQL Server / Sqlite)
- [x] `specs/001-identity/011-audit-logs` — CRM-036 Identity audit log API + admin-mfe list
- [x] `specs/001-identity/012-system-config` — CRM-037 admin system settings UI + lockout policy
- [x] `specs/005-channels/013-whatsapp-channel` — CRM-009 WhatsApp intake + reply (DevWhatsAppProvider)
- [x] `specs/005-channels/014-live-chat` — CRM-010 live chat intake + reply (DevChatProvider) + portal chat
- [x] `specs/005-channels/015-sms-channel` — CRM-011 SMS intake + reply (DevSmsProvider)
- [x] `specs/005-channels/016-channel-providers` — CRM-040 SendGrid + Twilio SMS/WhatsApp adapters
- [x] `specs/007-sla/017-sla-targets` — CRM-017 response/resolution SLA policies + ticket due/breach
- [x] `specs/007-sla/018-sla-automation` — CRM-018/019 auto-assign rules + escalation automation
- [x] `specs/008-knowledge/019-knowledge-authoring` — CRM-021 knowledge article authoring (FAQ/Article/Solution/Guide)
- [x] `specs/008-knowledge/020-knowledge-search` — CRM-022 ranked knowledge search + agent panel
- [x] `specs/009-notifications/021-notifications` — CRM-020 in-app notifications inbox + shell bell
- [x] `specs/008-knowledge/022-portal-faqs` — CRM-029 portal FAQ browse + detail
- [x] `specs/004-tickets/023-ticket-collaboration` — CRM-016 internal notes + @mentions
- [x] `specs/005-channels/024-inbound-webhooks` — CRM-040 Twilio inbound webhook signature verification
- [x] `specs/006-agent/025-agent-workspace` — CRM-013 my tickets + customer summary on detail
- [x] `specs/004-tickets/026-ticket-tasks` — CRM-014 ticket tasks + due-today home
- [x] `specs/006-agent/027-quick-replies` — CRM-015 shared quick reply catalog
- [x] `specs/010-portal/028-customer-feedback` — CRM-030 portal CSAT + agent read-only
- [x] `specs/011-reports/029-ticket-reports` — CRM-031 ticket volume reports
- [x] `specs/007-sla/030-sla-performance` — CRM-032 SLA / agent performance
- [x] `specs/011-reports/031-csat-reports` — CRM-033 CSAT aggregates
- [x] `specs/011-reports/032-management-dashboard` — CRM-034 admin KPI dashboard
- [x] `specs/012-ai/033-ai-ticket-summaries` — CRM-023 Nest AI + ticket summaries
- [x] `specs/012-ai/034-ai-suggested-replies` — CRM-024 suggested replies
- [x] `specs/012-ai/035-ai-auto-categorize` — CRM-025 auto-categorize
- [x] `specs/012-ai/036-ai-portal-chatbot` — CRM-026 portal AI chatbot
- [x] `specs/013-integrations/037-external-apis` — CRM-038 gateway external API key surface
- [x] `specs/001-identity/038-departments-branches` — CRM-043 departments and branches
- [x] `specs/001-identity/039-custom-branding` — CRM-044 custom branding
- [x] `specs/013-integrations/040-erp-webhook-stub` — CRM-039 ERP outbound webhook stub
- [x] `specs/013-integrations/041-external-openapi` — CRM-038 OpenAPI for external v1
- [x] `specs/012-ai/042-persist-ai-summary` — CRM-023 persist AI summaries on ticket
- [x] `specs/012-ai/043-chat-session-memory` — CRM-026 portal chat multi-turn memory
- [x] `specs/013-integrations/044-erp-webhook-retries` — CRM-039 ERP webhook retries + delivery log
- [x] `specs/013-integrations/045-external-swagger-ui` — CRM-038 Swagger UI for external v1
- [x] `specs/012-ai/046-stream-ai-summary` — CRM-023 streaming AI summaries
- [x] `specs/012-ai/047-durable-chat` — CRM-026 durable chat sessions + handoff
- [x] `specs/013-integrations/048-erp-outbox-auth` — CRM-039 ERP outbox UI + auth headers
- [x] `specs/001-identity/049-customer-register` — CRM-045 customer self-registration
- [x] `specs/001-identity/050-password-reset` — CRM-046 forgot / reset password

## Next

1. SendGrid inbound parse / Meta webhook signatures when ready
2. Further polish only if needed

## Constraints

- Spec before code (`specs/WORKFLOW.md`)
- Gateway-only public edge (`http://localhost:5000`)
- .NET vertical slice + CQRS + DDD; NestJS for channels / AI / realtime
- Angular Native Federation; MFEs never call downstream ports
- Demo user: `agent@crm.local` / `Crm!123` (also `admin@crm.local` for admin)
