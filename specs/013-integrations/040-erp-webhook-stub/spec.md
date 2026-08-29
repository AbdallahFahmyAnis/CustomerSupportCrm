# Feature Specification: ERP webhook stub

**Story**: CRM-039  
**Status**: Implemented  
**Created**: 2026-08-25

## Scope
- ErpWebhookUrl on SystemSettings (admin)
- On ticket create, best-effort POST JSON payload to URL
- Empty URL = no-op

## DoD
- [x] Test CRM-039 payload shape
