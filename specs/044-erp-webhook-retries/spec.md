# Feature Specification: ERP webhook retries

**Story**: CRM-039 (polish)  
**Status**: Implemented  
**Created**: 2026-08-25

## Scope
- On non-2xx / failure: up to 2 in-process retries with short delay
- Append delivery log (ticketId, status, at)
- `GET /api/tickets/integrations/erp-deliveries` (last N)
- Admin settings: last deliveries list
- Empty URL remains no-op

## DoD
- [x] Test payload + empty-URL no-op + retries (CRM-039)
- [x] Defer: full outbox UI, auth headers
