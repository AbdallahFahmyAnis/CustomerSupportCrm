# Feature Specification: Portal chat session memory

**Story**: CRM-026 (polish)  
**Status**: Implemented  
**Created**: 2026-08-25

## Scope
- In-memory session store in AI Nest keyed by `sessionId` (last ~6 turns)
- `POST /api/ai/chat` uses prior turns in heuristic
- portal-mfe keeps and resends `sessionId` from first response

## DoD
- [x] Multi-turn test cites CRM-026
- [x] Defer: durable DB, human handoff
