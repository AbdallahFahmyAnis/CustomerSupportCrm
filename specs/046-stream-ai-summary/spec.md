# Feature Specification: Streaming AI ticket summaries

**Story**: CRM-023 (deferred)  
**Status**: Implemented  
**Created**: 2026-08-25

## Scope
- `POST /api/ai/tickets/:id/summary/stream` SSE token events + final done payload
- Heuristic chunking; still persists summary after generate
- agent-mfe streams tokens into the summary panel

## DoD
- [x] Test cites CRM-023 for streamSummaryChunks
