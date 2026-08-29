# Feature Specification: Platform foundation

**Story**: CRM-041, CRM-042 (partial chrome); platform slice `001`  
**Epic**: Platform  
**Priority**: Must  
**Status**: Implemented  
**Created**: 2026-08-24

## User story

**As a** support agent,  
**I want** to open the CRM in my browser through a single address, see Arabic or English chrome, and reach a working agent workspace remote,  
**so that** later customer and ticket slices have a place to land.

## Business value

Without a gateway, identity stub, and shell, every later story would re-invent hosting, auth, and i18n.

## Scope

**In scope**
- Spec Kit SDD layout (already required by constitution)
- .NET Gateway BFF on `http://localhost:5000` (only public edge)
- Identity, Customers, Tickets health; Customers `GetBootstrapStatus` vertical-slice query
- NestJS Channels and Notifications health, proxied by the gateway
- Angular shell + Native Federation remotes (agent live; portal/admin/knowledge stub)
- Arabic/English shell chrome with RTL/LTR (CRM-041 start)
- Responsive shell layout (CRM-042 start)
- Dev login cookie: `agent@crm.local` / `Crm!123`

**Out of scope**
- Customer CRUD (CRM-001)
- Ticket lifecycle (CRM-004)
- Real roles admin (CRM-035)
- Knowledge, SLA, and AI APIs
- Message brokers, Mongo, extra containers
- Real email/WhatsApp providers

## Preconditions

- Local machine can run .NET 9, Node.js, and Angular CLI.
- Browser uses `http://localhost:5000` only.

## Screens

| Screen | URL | Actor | Observable result |
|---|---|---|---|
| Shell home | `http://localhost:5000/` | agent | Product name, language toggle, sign-in or signed-in chrome |
| Agent workspace | `http://localhost:5000/agent` | agent | agent-mfe remote loaded (“workspace ready” placeholder) |
| Portal stub | `http://localhost:5000/portal` | customer | “coming soon” from portal-mfe |
| Admin stub | `http://localhost:5000/admin` | admin | “coming soon” from admin-mfe |
| Knowledge stub | `http://localhost:5000/knowledge` | author | “coming soon” from knowledge-mfe |

## Acceptance criteria

1. **Given** the platform is running, **when** I open `http://localhost:5000/`, **then** I see the shell with English labels by default and a language toggle.
2. **Given** I choose Arabic, **when** the UI updates, **then** labels are Arabic and layout is right-to-left.
3. **Given** I choose English again, **when** the UI updates, **then** labels are English and layout is left-to-right.
4. **Given** I am not signed in, **when** I POST valid demo credentials, **then** I receive a BFF cookie and the shell shows the agent name.
5. **Given** I am signed in, **when** I open `/agent`, **then** the agent remote loads without calling `localhost:510x` from the browser.
6. **Given** the gateway is up, **when** I GET `/health`, **then** Identity, Customers, Channels, and Notifications report status.
7. **Given** Customers is up, **when** I GET `/api/customers/bootstrap` through the gateway, **then** I receive JSON from the `GetBootstrapStatus` query (VSA + CQRS).
8. **Given** portal, admin, or knowledge routes, **when** I open them, **then** I see a coming-soon remote, not a gateway error.

## Definition of Done

- [x] Acceptance criteria pass in UAT on the gateway
- [x] Automated test `[Trait("Story", "CRM-041")]` for bootstrap/health where applicable
- [x] Demo login makes the shell clickable
- [x] Spec, plan, and code cite `001-platform-foundation` / CRM-041

## Assumptions and dependencies

- Depends on: none (first slice)
- Assumptions: SQLite unused except as a placeholder; health is enough for Channels/Notifications; Knowledge/SLA/AI stay documented only
