# Feature Specification: Knowledge base search

**Story**: CRM-022  
**Epic**: Knowledge Base  
**Priority**: Should  
**Status**: Implemented  
**Created**: 2026-08-25

## User story

**As a** support agent,  
**I want** to search FAQs, articles, solutions, and guides by keyword with useful ranking and filters,  
**so that** I can find an approved answer while working a ticket without scrolling the full catalog.

## Business value

Speeds ticket handling by putting reusable answers in front of agents at the moment of need.

## Scope

**In scope**
- CRM-022: Ranked search API over Knowledge articles (title matches rank above body; optional kind/status filters; published-only mode)
- Result hits include score and a short body snippet around the first match
- knowledge-mfe **Search** screen with query + kind/status filters
- Agent ticket detail: compact knowledge search panel (opens hit in knowledge or shows snippet)
- Automated tests with `[Trait("Story", "CRM-022")]`
- Remains on EF/Sqlite (Mongo full-text deferred)

**Out of scope**
- CRM-029 portal FAQ browsing
- Semantic / vector / AI search (CRM-024)
- MongoDB Atlas Search / Elasticsearch

## Preconditions

- `specs/019-knowledge-authoring` (CRM-021) articles API + seed
- Demo: `agent@crm.local` / `Crm!123` at `http://localhost:5000`

## Screens

| Screen | URL | Actor | Observable result |
|---|---|---|---|
| Knowledge search | `http://localhost:5000/knowledge/search` | agent / author | Query returns ranked hits with kind/status filters |
| Agent ticket detail | `http://localhost:5000/agent/tickets/{id}` | agent | Knowledge panel searches published articles and shows snippets |

## Acceptance criteria

1. **Given** seeded articles, **when** search `q=password`, **then** the FAQ about portal password ranks at or near the top with a snippet.
2. **Given** `kind=Solution`, **when** search runs, **then** only Solution hits are returned.
3. **Given** `publishedOnly=true`, **when** Draft articles match the query, **then** they are excluded.
4. **Given** empty `q`, **when** search is called, **then** the API returns a clear validation error (or empty list — choose reject with 400).
5. **Given** an agent on ticket detail, **when** they search from the knowledge panel, **then** published hits appear without leaving the ticket.

## Definition of Done

- [x] AC pass on gateway UAT
- [x] Automated test `[Trait("Story", "CRM-022")]`
- [x] Seed + search screens clickable
- [x] Spec/plan/code cite CRM-022
- [x] Product Next updated

## Assumptions and dependencies

- Depends on: CRM-021
- Assumptions: ranking is lexical (title/body contains), not ML
