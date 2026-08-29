# Feature Specification: Persist AI ticket summaries

**Story**: CRM-023 (polish)  
**Status**: Implemented  
**Created**: 2026-08-25

## Scope
- Tickets: `AiSummary` / `AiSummaryAt` (+ highlights JSON) on ticket row
- `PUT /api/tickets/{id}/ai-summary` body `{ summary, highlights? }`
- AI generate-summary persists via Tickets HTTP after heuristic
- agent-mfe shows saved summary on ticket detail load

## DoD
- [x] Test cites CRM-023; put + get round-trip
- [x] Defer: streaming
