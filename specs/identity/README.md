# Feature: Identity & Access

**Slug**: `identity`  
**Status**: Active (multi-slice)  
**Owning service**: Identity (+ shell / admin-mfe / portal for UX)  
**Created**: 2026-08-29

## Purpose

One place for everything that authenticates users, authorizes roles/permissions, and manages portal/staff accounts. Child slices stay as numbered `specs/NNN-slug` folders (SDD workflow); this file is the **feature map**.

## Stories in this feature

| Story | Slice | Status | Summary |
|---|---|---|---|
| CRM-035 | [004-identity-admin](../004-identity-admin/spec.md) | Implemented | Users, roles, permissions, sign-in tokens |
| CRM-035 / CRM-037 | [007-identity-ef-core](../007-identity-ef-core/spec.md) | Implemented | OpenIddict + ASP.NET Identity + EF Core |
| CRM-036 | [011-audit-logs](../011-audit-logs/spec.md) | Implemented | Admin audit log list |
| CRM-037 | [012-system-config](../012-system-config/spec.md) | Implemented | System settings + lockout policy |
| CRM-043 | [038-departments-branches](../038-departments-branches/spec.md) | Implemented | Departments and branches |
| CRM-044 | [039-custom-branding](../039-custom-branding/spec.md) | Implemented | Branding (Identity settings + shell) |
| CRM-045 | [049-customer-register](../049-customer-register/spec.md) | Draft | Customer self-registration |
| CRM-046 | [050-password-reset](../050-password-reset/spec.md) | Draft | Forgot password / password reset |

## Related but not owned here

- `006-data-platform` — SQL Server/Postgres engines Identity uses  
- Portal submit/track (`005`, `027`–`030`) — consume Customer sessions after Identity login  
- Channels SendGrid (`016` / `009`) — delivery for reset emails when configured  

## Active draft order

1. **CRM-045** — register (`049`)  
2. **CRM-046** — password reset (`050`)  

## Conventions

- Spec before code; cite `CRM-nnn` in plan/code/tests  
- Gateway-only public edge; browsers never call Identity `:5101`  
- Customer self-service never elevates to Agent/Admin/Lead  
- Demo seed: `agent@crm.local` / `admin@crm.local` / `customer@crm.local` + `Crm!123`
