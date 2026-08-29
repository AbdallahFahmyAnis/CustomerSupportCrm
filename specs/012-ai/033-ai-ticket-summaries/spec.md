# Feature Specification: AI ticket summaries

**Story**: CRM-023  
**Epic**: AI Features  
**Priority**: Could  
**Status**: Implemented  
**Created**: 2026-08-25

## User story

**As a** support agent,  
**I want** an AI-generated summary of a ticket,  
**so that** I can grasp context quickly.

## Scope

- Nest AI service :5203 + gateway `/api/ai`
- `POST /api/ai/tickets/:id/summary` (heuristic provider)
- agent-mfe summary panel on ticket detail

## Definition of Done

- [x] AC + npm test CRM-023
- [x] Spec/plan/code cite CRM-023
