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

## Next

1. Channels / portal
2. CRM-036 audit logs / CRM-037 system config

## Constraints

- Spec before code (`specs/WORKFLOW.md`)
- Gateway-only public edge (`http://localhost:5000`)
- .NET vertical slice + CQRS + DDD; NestJS for channels / AI / realtime
- Angular Native Federation; MFEs never call downstream ports
- Demo user: `agent@crm.local` / `Crm!123` (also `admin@crm.local` for admin)
