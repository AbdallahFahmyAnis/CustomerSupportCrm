# Feature Specification: Durable chat sessions + handoff

**Story**: CRM-026 (deferred)  
**Status**: Implemented  
**Created**: 2026-08-25

## Scope
- File-backed chat session store (survives AI service restart)
- Detect human handoff keywords; `handoffNeeded` on chat response
- portal-mfe shows handoff CTA to submit a request

## DoD
- [x] Test cites CRM-026 for persist + handoff detect
