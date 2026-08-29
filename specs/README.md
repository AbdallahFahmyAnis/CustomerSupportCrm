# Specs layout

Product contract: [`000-product/spec.md`](000-product/spec.md)  
Backlog: [`STORIES.md`](STORIES.md) · Workflow: [`WORKFLOW.md`](WORKFLOW.md)

## Feature hubs (ordered)

Each hub is `NNN-slug/` with `feature.md` (map) and numbered story slices `MMM-story-slug/` underneath.

| # | Hub | Path | Domain |
|---|---|---|---|
| 001 | Identity | [`001-identity/`](001-identity/feature.md) | Auth, users, audit, settings, register/reset |
| 002 | Platform | [`002-platform/`](002-platform/feature.md) | Foundation, data engines, shared EF |
| 003 | Customers | [`003-customers/`](003-customers/feature.md) | Profiles, contacts, notes |
| 004 | Tickets | [`004-tickets/`](004-tickets/feature.md) | Lifecycle, collaboration, tasks |
| 005 | Channels | [`005-channels/`](005-channels/feature.md) | Email, WhatsApp, chat, SMS, providers, webhooks |
| 006 | Agent | [`006-agent/`](006-agent/feature.md) | Agent workspace, quick replies |
| 007 | SLA | [`007-sla/`](007-sla/feature.md) | Targets, automation, performance |
| 008 | Knowledge | [`008-knowledge/`](008-knowledge/feature.md) | Authoring, search, portal FAQs |
| 009 | Notifications | [`009-notifications/`](009-notifications/feature.md) | In-app alerts |
| 010 | Portal | [`010-portal/`](010-portal/feature.md) | Customer feedback (+ related portal UX) |
| 011 | Reports | [`011-reports/`](011-reports/feature.md) | Ticket / CSAT / management dashboards |
| 012 | AI | [`012-ai/`](012-ai/feature.md) | Summaries, suggestions, chatbot |
| 013 | Integrations | [`013-integrations/`](013-integrations/feature.md) | External APIs, ERP, OpenAPI |

## Active slice

`.specify/feature.json` → `feature_directory` = `specs/NNN-hub/MMM-slug`, optional `feature_family` = `specs/NNN-hub`.

New slices: next global `MMM` under the owning hub, e.g. `specs/001-identity/052-slug/spec.md`.

## Legacy slice paths

Pre-hub bookmarks like `specs/016-channel-providers/README.md` redirect to the Channels hub (`specs/005-channels/...`).
