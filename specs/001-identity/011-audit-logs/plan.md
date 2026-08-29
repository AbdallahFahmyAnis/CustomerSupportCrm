# Implementation Plan: Audit logs

**Spec**: `specs/001-identity/011-audit-logs/spec.md`  
**Story**: CRM-036  

## Summary

Add `AuditLogs` EF table to Identity, write events from auth/admin handlers, expose admin list API, admin-mfe audit feature.

## Code to apply

| Area | Path |
|---|---|
| EF | `AuditLogEntry` + DbContext config; EnsureSchema create-if-missing |
| API | `Features/Audit/ListAuditLogs/*` |
| Write | IssueToken, CreateUser, UpdateUserRole, DeactivateUser handlers |
| Contract | `AuditLogDto` |
| FE | `admin-mfe/features/audit/*` + home link + remote route |
| Test | IdentityAdminTests CRM-036 |

## Constitution Check

- [x] One slice; gateway-only; Identity owns audit
