# Implementation Plan: Audit log detail

**Spec**: `specs/001-identity/052-audit-detail/spec.md`  
**Story**: CRM-036 (extend)  
**Workflow stage after this plan**: Apply → Test → Mock → Done

## Summary

Add Identity query/handler for a single audit row with resolved actor/target display fields; expose `GET /api/identity/audit/{id}`; admin-mfe detail route and page; list links to detail.

## Code to apply

| Area | Path | Story |
|---|---|---|
| Contracts | `AuditLogDetailDto` | CRM-036 |
| API | `Features/Audit/GetAuditLog/*` | CRM-036 |
| admin-mfe | `features/audit/audit-detail/*`, list “View details” | CRM-036 |
| Tests | `IdentityAdminTests` GET detail + 404 | CRM-036 |
