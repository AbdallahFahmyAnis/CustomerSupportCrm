# Customer Support CRM Constitution

Non-negotiable rules for every spec, plan, and implementation. If a slice conflicts with this file, change the spec or amend the constitution — do not silently ignore it.

## Core Principles

### Spec before code

New product slices start with `specs/<hub>/NNN-slug/spec.md` (hub map in `specs/README.md`). Do not implement a slice from a chat prompt alone. "Proceed to next" means: pick or write the next spec, then plan and implement that spec.

### Story traceability

Every shipped slice has a story id `CRM-nnn` matching Azure DevOps. Product order and BA user stories are in `specs/STORIES.md`. Specs include **screens**, **apply (code paths)**, **test**, and **mock**. New types and endpoint classes cite the story in XML docs or file headers. Tests use `[Trait("Story", "CRM-nnn")]`. Workflow: Specify → Apply → Test → Mock → Retest → Done (`specs/WORKFLOW.md`).

### One vertical slice

Each spec is one demoable capability (API + UI the user can click). Do not mix unrelated services in the same slice. Prefer the owning bounded context unless the spec clearly belongs to another service.

### Gateway is the only public edge

Browsers talk to `http://localhost:5000`. Downstream APIs stay on 510x (.NET) and 520x (NestJS). Micro-frontend remotes are also reached through the gateway (`/mfe/...`). Do not expose a microservice or remote port as the agent/customer UI. Session is the BFF cookie; the gateway attaches identity to downstream calls.

### Own data in the owning service

Customers owns profiles, contacts, and customer attachments. Tickets owns ticket lifecycle. Identity owns users, roles, and permissions. Knowledge owns articles. SLA owns policies and timers. Channels owns inbound/outbound messages. Notifications owns in-app alerts. AI owns model calls. Do not duplicate another service's source of truth.

### Vertical slice + CQRS + DDD

.NET features use `Features/{Area}/{UseCase}/` with `Endpoint` / `Command|Query` / `Handler` (see `.cursor/rules/dotnet-vertical-slice.mdc`).  
NestJS services use `features/{area}/{use-case}/` with `route.ts` / `schema.ts` / `handler.ts` (+ optional `service.ts`), plus `domain/`, `infrastructure/`, `shared/`, and `app/` + `server.ts` (see `.cursor/rules/nestjs-service-structure.mdc`).  
In-process CQRS (MediatR / `@nestjs/cqrs`) is required for new write/read use cases. Integration events are contracts; do not add a message broker unless the spec names it. Aggregates and value objects live in the owning service `domain/` (or .NET `Domain/`).

### Keep local persistence boring

.NET services prefer **EF Core** (SQL Server for Identity/Customers/Tickets; Sqlite escape hatch for offline tests). Nest Channels prefers **TypeORM on PostgreSQL** (`CHANNELS_DATABASE_URL`) with JSON file escape hatch. Spec `006-data-platform` Docker SQL Server / Postgres / Mongo remain the local/CI engines; Knowledge → Mongo later. Spec `007-identity-ef-core` requires OpenIddict + ASP.NET Core Identity on EF Core. Seed must not take down startup. Prefer connection strings from config/env over committed secrets.

### Small, related commits

Stage only files for the slice. Never commit `.tmp-build/`, `*.db`, `bin/`, `obj/`, or `node_modules/`. Default password in docs stays `Crm!123`.

## Development Constraints

- Angular: Native Federation. Shell owns chrome, language, and auth. agent-mfe owns agent work; portal-mfe owns customer portal; admin-mfe owns security/config; knowledge-mfe owns authoring/search.
- MFEs call `/api/...` on the gateway only — never `localhost:510x` or `localhost:520x`.
- Frontend structure is **Feature-Based + Signals** under `src/app/`:
  - `core/` — auth, HTTP interceptors, app providers
  - `shared/` — reusable local UI/utilities (cross-MFE kits stay in `projects/shared`)
  - `layout/` — shell chrome when the app owns it
  - `features/{feature}/{use-case}/` — screens as `{name}.page.ts` + `{name}.html` + `{name}.scss`; feature root holds `{feature}.api.ts`, `{feature}.models.ts`, `{feature}.store.ts`, `{feature}.routes.ts`
  - Feature state uses Angular **signals** / a small feature or page store — not NgRx for new work
  - Smart pages call the feature/page store; presentational widgets take `input` / `output` only
  - No inline `template` / `styles` for UI components
- Arabic (RTL) and English (LTR) shell chrome must keep working.
- Do not introduce RabbitMQ or unrelated containers unless the spec names them. Docker SQL Server / Postgres / Mongo from `006-data-platform` are allowed.

## Governance

- Amend this constitution in the same change that violates it, with a one-line reason in the spec's Constitution Check.
- Specs are the review surface: if it is not in `spec.md`, it is not required.
- After implement, mark tasks done and set spec status to Implemented.

**Version**: 1.5.0 | **Ratified**: 2026-08-24 | **Last Amended**: 2026-08-24 — OpenIddict + EF Core Identity; Channels TypeORM/Postgres (007)
