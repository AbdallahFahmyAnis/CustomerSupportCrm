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

- [x] `specs/platform/001-platform-foundation` — gateway, bilingual shell, health, VSA example
- [x] `specs/customers/002-customer-profiles` — CRM-001…003 profiles, contacts, notes/attachments
- [x] `specs/tickets/003-ticket-lifecycle` — CRM-004…007 tickets (Feature-Based + Signals FE)
- [x] `specs/identity/004-identity-admin` — CRM-035 users, roles, permissions
- [x] `specs/channels/005-channels-portal` — CRM-012 / CRM-027 / CRM-028 web form + portal submit/track
- [x] `specs/platform/006-data-platform` — Docker SQL Server / Postgres / Mongo + Azure Pipelines; Identity SQL path
- [x] `specs/identity/007-identity-ef-core` — OpenIddict + ASP.NET Identity + EF Core; Channels TypeORM/Postgres
- [x] `specs/channels/008-email-channel` — CRM-008 email intake + CRM-040 DevEmailProvider stub
- [x] `specs/channels/009-email-outbound` — CRM-040 outbound email reply (dev + optional SMTP)
- [x] `specs/platform/010-customers-tickets-ef` — Customers/Tickets EF Core (SQL Server / Sqlite)
- [x] `specs/identity/011-audit-logs` — CRM-036 Identity audit log API + admin-mfe list
- [x] `specs/identity/012-system-config` — CRM-037 admin system settings UI + lockout policy
- [x] `specs/channels/013-whatsapp-channel` — CRM-009 WhatsApp intake + reply (DevWhatsAppProvider)
- [x] `specs/channels/014-live-chat` — CRM-010 live chat intake + reply (DevChatProvider) + portal chat
- [x] `specs/channels/015-sms-channel` — CRM-011 SMS intake + reply (DevSmsProvider)
- [x] `specs/channels/016-channel-providers` — CRM-040 SendGrid + Twilio SMS/WhatsApp adapters
- [x] `specs/sla/017-sla-targets` — CRM-017 response/resolution SLA policies + ticket due/breach
- [x] `specs/sla/018-sla-automation` — CRM-018/019 auto-assign rules + escalation automation
- [x] `specs/knowledge/019-knowledge-authoring` — CRM-021 knowledge article authoring (FAQ/Article/Solution/Guide)
- [x] `specs/knowledge/020-knowledge-search` — CRM-022 ranked knowledge search + agent panel
- [x] `specs/notifications/021-notifications` — CRM-020 in-app notifications inbox + shell bell
- [x] `specs/knowledge/022-portal-faqs` — CRM-029 portal FAQ browse + detail
- [x] `specs/tickets/023-ticket-collaboration` — CRM-016 internal notes + @mentions
- [x] `specs/channels/024-inbound-webhooks` — CRM-040 Twilio inbound webhook signature verification
- [x] `specs/agent/025-agent-workspace` — CRM-013 my tickets + customer summary on detail
- [x] `specs/tickets/026-ticket-tasks` — CRM-014 ticket tasks + due-today home
- [x] `specs/agent/027-quick-replies` — CRM-015 shared quick reply catalog
- [x] `specs/portal/028-customer-feedback` — CRM-030 portal CSAT + agent read-only
- [x] `specs/reports/029-ticket-reports` — CRM-031 ticket volume reports
- [x] `specs/sla/030-sla-performance` — CRM-032 SLA / agent performance
- [x] `specs/reports/031-csat-reports` — CRM-033 CSAT aggregates
- [x] `specs/reports/032-management-dashboard` — CRM-034 admin KPI dashboard
- [x] `specs/ai/033-ai-ticket-summaries` — CRM-023 Nest AI + ticket summaries
- [x] `specs/ai/034-ai-suggested-replies` — CRM-024 suggested replies
- [x] `specs/ai/035-ai-auto-categorize` — CRM-025 auto-categorize
- [x] `specs/ai/036-ai-portal-chatbot` — CRM-026 portal AI chatbot
- [x] `specs/integrations/037-external-apis` — CRM-038 gateway external API key surface
- [x] `specs/identity/038-departments-branches` — CRM-043 departments and branches
- [x] `specs/identity/039-custom-branding` — CRM-044 custom branding
- [x] `specs/integrations/040-erp-webhook-stub` — CRM-039 ERP outbound webhook stub
- [x] `specs/integrations/041-external-openapi` — CRM-038 OpenAPI for external v1
- [x] `specs/ai/042-persist-ai-summary` — CRM-023 persist AI summaries on ticket
- [x] `specs/ai/043-chat-session-memory` — CRM-026 portal chat multi-turn memory
- [x] `specs/integrations/044-erp-webhook-retries` — CRM-039 ERP webhook retries + delivery log
- [x] `specs/integrations/045-external-swagger-ui` — CRM-038 Swagger UI for external v1
- [x] `specs/ai/046-stream-ai-summary` — CRM-023 streaming AI summaries
- [x] `specs/ai/047-durable-chat` — CRM-026 durable chat sessions + handoff
- [x] `specs/integrations/048-erp-outbox-auth` — CRM-039 ERP outbox UI + auth headers
- [x] `specs/identity/049-customer-register` — CRM-045 customer self-registration
- [x] `specs/identity/050-password-reset` — CRM-046 forgot / reset password

## Next

1. SendGrid inbound parse / Meta webhook signatures when ready
2. Further polish only if needed

## Constraints

- Spec before code (`specs/WORKFLOW.md`)
- Gateway-only public edge (`http://localhost:5000`)
- .NET vertical slice + CQRS + DDD; NestJS for channels / AI / realtime
- Angular Native Federation; MFEs never call downstream ports
- Demo user: `agent@crm.local` / `Crm!123` (also `admin@crm.local` for admin)
