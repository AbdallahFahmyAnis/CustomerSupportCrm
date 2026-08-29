# Specs layout

Product contract: [`000-product/spec.md`](000-product/spec.md)  
Backlog: [`STORIES.md`](STORIES.md) · Workflow: [`WORKFLOW.md`](WORKFLOW.md)

## Feature hubs

Each hub has `feature.md` (map) and numbered story slices `NNN-slug/` underneath.

| Hub | Path | Domain |
|---|---|---|
| Platform | [`platform/`](platform/feature.md) | Foundation, data engines, shared EF |
| Customers | [`customers/`](customers/feature.md) | Profiles, contacts, notes |
| Tickets | [`tickets/`](tickets/feature.md) | Lifecycle, collaboration, tasks |
| Identity | [`identity/`](identity/feature.md) | Auth, users, audit, settings, register/reset |
| Channels | [`channels/`](channels/feature.md) | Email, WhatsApp, chat, SMS, providers, webhooks |
| Agent | [`agent/`](agent/feature.md) | Agent workspace, quick replies |
| SLA | [`sla/`](sla/feature.md) | Targets, automation, performance |
| Knowledge | [`knowledge/`](knowledge/feature.md) | Authoring, search, portal FAQs |
| Notifications | [`notifications/`](notifications/feature.md) | In-app alerts |
| Portal | [`portal/`](portal/feature.md) | Customer feedback (+ related portal UX) |
| Reports | [`reports/`](reports/feature.md) | Ticket / CSAT / management dashboards |
| AI | [`ai/`](ai/feature.md) | Summaries, suggestions, chatbot |
| Integrations | [`integrations/`](integrations/feature.md) | External APIs, ERP, OpenAPI |

## Active slice

`.specify/feature.json` → `feature_directory` = `specs/<hub>/NNN-slug`, optional `feature_family` = `specs/<hub>`.

New slices: next `NNN` under the owning hub, e.g. `specs/identity/052-slug/spec.md`.
