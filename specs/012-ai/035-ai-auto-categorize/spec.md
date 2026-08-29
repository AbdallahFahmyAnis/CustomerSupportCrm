# Feature Specification: AI auto-categorize

**Story**: CRM-025  
**Epic**: AI Features  
**Priority**: Could  
**Status**: Implemented  
**Created**: 2026-08-25

## User story

**As a** support agent,  
**I want** AI to suggest category and priority for a ticket,  
**so that** I can classify faster with one click.

## Scope

- `POST /api/ai/tickets/:id/categorize` → `{ category, priority, confidence }` (heuristic)
- agent-mfe: show suggestion + **Apply** via existing `PUT /api/tickets/{id}/classification`
- Categories/priorities aligned to `TicketCatalog`

## Out of scope

- Auto-apply on create without agent confirm

## Definition of Done

- [x] AC + npm test CRM-025
- [x] Spec/plan/code cite CRM-025
