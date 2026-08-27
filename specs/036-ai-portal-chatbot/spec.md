# Feature Specification: Portal AI chatbot

**Story**: CRM-026  
**Epic**: AI Features  
**Priority**: Could  
**Status**: Implemented  
**Created**: 2026-08-25

## User story

**As a** portal customer,  
**I want** an AI assistant for common questions,  
**so that** I can self-serve before opening a ticket.

## Scope

- `POST /api/ai/chat` body `{ message, sessionId? }` → `{ reply, sources? }`
- Heuristic FAQ match via Knowledge portal list + canned fallback
- portal-mfe: `/portal/assistant` Q&A UI + home tile

## Out of scope

- Multi-turn memory store, human handoff

## Definition of Done

- [x] AC + npm test CRM-026
- [x] Spec/plan/code cite CRM-026
