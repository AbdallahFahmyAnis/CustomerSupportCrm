# Feature Specification: Knowledge article authoring

**Story**: CRM-021  
**Epic**: Knowledge Base  
**Priority**: Should  
**Status**: Implemented  
**Created**: 2026-08-25

## User story

**As a** knowledge author,  
**I want** to create and edit FAQs, articles, solutions, and guides,  
**so that** agents and customers can reuse approved answers instead of rewriting them.

## Business value

Centralizes reusable content so support quality stays consistent and agents spend less time reinventing replies.

## Scope

**In scope**
- CRM-021: New **Knowledge** .NET service (`:5104`) owning articles
- Article kinds: FAQ, Article, Solution, Guide; statuses Draft / Published
- List (optional text filter), get, create, update; seed demo articles
- knowledge-mfe: list + create/edit screens at `/knowledge`
- Gateway proxy `/api/knowledge/...` + health probe
- EF Core Sqlite locally (SQL Server optional); Mongo deferred with CRM-022 search scale-out

**Out of scope**
- CRM-022 full-text / ranked search UX
- CRM-029 portal FAQ surface
- Attachments / rich media CMS
- AI suggested articles (CRM-024)

## Preconditions

- knowledge-mfe remote already routed from shell (`/knowledge`)
- Demo: `agent@crm.local` / `Crm!123` at `http://localhost:5000`

## Screens

| Screen | URL | Actor | Observable result |
|---|---|---|---|
| Knowledge home / list | `http://localhost:5000/knowledge` | agent / author | Seeded articles listed; filter by title text |
| Create article | `http://localhost:5000/knowledge/new` | author | Can save Draft or Published FAQ/Article/Solution/Guide |
| Edit article | `http://localhost:5000/knowledge/{id}` | author | Can change title, body, kind, status and save |

## Acceptance criteria

1. **Given** a fresh Knowledge DB, **when** the service starts, **then** at least two seeded articles exist (one FAQ, one Solution).
2. **Given** an authenticated user, **when** they open `/knowledge`, **then** they see the article list from the API.
3. **Given** valid create input, **when** they save a new article, **then** it appears in the list with the chosen kind and status.
4. **Given** an existing article, **when** they edit title/body/status, **then** GET by id reflects the changes.
5. **Given** missing title or unknown kind/status, **when** create/update is called, **then** the API rejects with a clear error.
6. **Given** gateway + Knowledge running, **when** `GET /health` runs, **then** aggregate includes `knowledge` as ok.

## Definition of Done

- [x] AC pass on gateway UAT
- [x] Automated test `[Trait("Story", "CRM-021")]`
- [x] Seed makes list clickable
- [x] Spec/plan/code cite CRM-021
- [x] Product Next updated

## Assumptions and dependencies

- Depends on: platform shell + knowledge-mfe stub (001)
- Assumptions: any signed-in CRM user may author in this slice (finer permissions later)
