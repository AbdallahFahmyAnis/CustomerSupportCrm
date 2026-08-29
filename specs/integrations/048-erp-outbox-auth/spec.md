# Feature Specification: ERP outbox + auth headers

**Story**: CRM-039 (deferred)  
**Status**: Implemented  
**Created**: 2026-08-25

## Scope
- Optional ERP webhook Authorization header on system settings
- Durable outbox file for delivery log (survives Tickets restart)
- Admin settings: auth field + outbox table UI

## DoD
- [x] Test cites CRM-039 for auth header on delivery
