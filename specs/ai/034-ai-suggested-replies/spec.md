# Feature Specification: AI suggested replies

**Story**: CRM-024  
**Status**: Implemented  
**Created**: 2026-08-25

## User story
As an agent, I want AI suggested replies so I can respond faster.

## Scope
- POST /api/ai/tickets/:id/suggestions
- agent-mfe insert into email/chat drafts

## DoD
- [x] npm test CRM-024 + UI
