# Feature: Identity & Access

**Slug**: `001-identity`  
**Status**: Active (multi-slice)  
**Owning service**: Identity (+ shell / admin-mfe / portal for UX)  
**Created**: 2026-08-29

## Purpose

Authenticates users, authorizes roles/permissions, and manages portal/staff accounts. Story slices live under this folder.

## Stories in this feature

| Story | Slice | Status | Summary |
|---|---|---|---|
| CRM-035 | [004-identity-admin](004-identity-admin/spec.md) | Implemented | Users, roles, permissions, sign-in tokens |
| CRM-035 / CRM-037 | [007-identity-ef-core](007-identity-ef-core/spec.md) | Implemented | OpenIddict + ASP.NET Identity + EF Core |
| CRM-036 | [011-audit-logs](011-audit-logs/spec.md) + [051-audit-cross-service](051-audit-cross-service/spec.md) | Implemented | Admin audit log; cross-service ingest + pagination |
| CRM-037 | [012-system-config](012-system-config/spec.md) | Implemented | System settings + lockout policy |
| CRM-043 | [038-departments-branches](038-departments-branches/spec.md) | Implemented | Departments and branches |
| CRM-044 | [039-custom-branding](039-custom-branding/spec.md) | Implemented | Branding (Identity settings + shell) |
| CRM-045 | [049-customer-register](049-customer-register/spec.md) | Implemented | Customer self-registration |
| CRM-046 | [050-password-reset](050-password-reset/spec.md) | Implemented | Forgot password / password reset |

## Related but not owned here

- `002-platform/006-data-platform` — SQL engines Identity uses  
- Portal submit/track (`005-channels/005`) — consume Customer sessions after Identity login  
- Channels SendGrid (`005-channels/016` / `009`) — delivery for reset emails when configured  

## Active draft order

_(none — CRM-045 / CRM-046 shipped)_

## Conventions

- Spec before code; cite `CRM-nnn` in plan/code/tests  
- Gateway-only public edge; browsers never call Identity `:5101`  
- Customer self-service never elevates to Agent/Admin/Lead  
- Demo seed: `agent@crm.local` / `admin@crm.local` / `customer@crm.local` + `Crm!123`
