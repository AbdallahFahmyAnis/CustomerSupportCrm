# Feature Specification: Portal FAQ access

**Story**: CRM-029  
**Epic**: Customer Portal  
**Priority**: Should  
**Status**: Implemented  
**Created**: 2026-08-25

## User story

**As a** customer using the portal,  
**I want** to browse and open published FAQs,  
**so that** I can answer common questions without opening a support ticket.

## Business value

Deflects repetitive tickets by putting approved self-service answers in the portal.

## Scope

**In scope**
- CRM-029: Portal-safe FAQ list and detail (Published + kind Faq only)
- Optional keyword filter on the FAQ list
- portal-mfe FAQ list + detail screens linked from the portal home
- Automated tests with `[Trait("Story", "CRM-029")]`
- Seed already provides at least one Published FAQ (CRM-021)

**Out of scope**
- Authoring (CRM-021) or agent ranked search UI (CRM-022)
- Draft / Solution / Guide / Article kinds on the portal
- Customer feedback (CRM-030)
- Unauthenticated public site without the shell (portal remains behind the gateway)

## Preconditions

- `specs/019-knowledge-authoring` (CRM-021) articles + seed
- Demo: `agent@crm.local` / `Crm!123` at `http://localhost:5000`

## Screens

| Screen | URL | Actor | Observable result |
|---|---|---|---|
| Portal home | `http://localhost:5000/portal` | customer / agent | FAQs tile navigates to FAQ list |
| FAQ list | `http://localhost:5000/portal/faqs` | customer / agent | Published FAQs listed; optional filter |
| FAQ detail | `http://localhost:5000/portal/faqs/{id}` | customer / agent | Full FAQ body; Draft/non-Faq not shown |

## Acceptance criteria

1. **Given** seeded knowledge, **when** the portal FAQ list loads, **then** at least the “portal password” FAQ appears and non-Faq kinds do not.
2. **Given** a Draft FAQ exists, **when** the portal list or detail is requested, **then** Drafts are excluded (404 on detail).
3. **Given** the customer filters by `password`, **when** the list refreshes, **then** matching Published FAQs remain and unrelated titles drop out.
4. **Given** the customer opens a FAQ from the list, **when** detail loads, **then** title and full body are visible.

## Definition of Done

- [x] Acceptance criteria pass in UAT on the gateway
- [x] Automated test `[Trait("Story", "CRM-029")]`
- [x] Mock/seed data makes the screens clickable
- [x] Spec, plan, and code cite `CRM-029`

## Assumptions and dependencies

- Depends on: CRM-021
- Assumptions: portal callers use the same gateway session as other portal screens; FAQ content is not personalized
