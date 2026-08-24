# Implementation Plan: Identity admin

**Spec**: `specs/004-identity-admin/spec.md`  
**Story**: CRM-035  
**Workflow stage after this plan**: Apply → Test → Mock → Done

## Summary

Replace hardcoded Identity login with SQLite-backed users/roles/permissions (VSA + CQRS). Gateway login keeps calling `/api/identity/dev-login` but that command authenticates against the store. admin-mfe gets `features/users` and `features/roles` (Feature-Based + Signals, separate html/scss/ts).

## Technical Context

**Language/Version**: .NET 9 / Angular 19  
**Edge**: `http://localhost:5000`  
**Owning service**: Identity (`Crm.Identity.Api` :5101)  
**Owning MFE**: admin-mfe

## Constitution Check

- [x] Spec exists with screens + AC
- [x] One vertical slice (Identity + admin-mfe)
- [x] No new public port
- [x] Story id in new types/endpoints
- [x] Command/query in feature folders
- [x] FE Feature-Based + Signals + html/scss/ts

## Seed

| Email | Role | Password |
|---|---|---|
| `agent@crm.local` | Agent | `Crm!123` |
| `admin@crm.local` | Admin | `Crm!123` |
| `lead@crm.local` | Lead | `Crm!123` |

Roles → permissions (demo catalog):  
- Admin: `users.manage`, `roles.view`, `tickets.*`, `customers.*`  
- Lead: `tickets.*`, `customers.*`, `tickets.assign`  
- Agent: `tickets.work`, `customers.read`

Stable Guid ids keep ticket assignment demo agent id `11111111-1111-1111-1111-111111111111`.

## Frontend layout

```
src/frontend/projects/admin-mfe/src/app/features/users/
  data-access/ users.api.ts, users.store.ts, user.models.ts
  pages/ user-list.page.*, user-create.page.*
  users.routes.ts
src/frontend/projects/admin-mfe/src/app/features/roles/
  data-access/ roles.api.ts, roles.store.ts
  pages/ role-list.page.*
  roles.routes.ts
```

## Code to apply

| Area | Path | Story |
|---|---|---|
| Domain/Infra | `src/services/identity/Crm.Identity.Api/` | CRM-035 |
| Contracts | `contracts/csharp/Crm.Contracts/Identity/` | CRM-035 |
| Tests | `tests/Crm.Identity.Api.Tests/` | CRM-035 |
| UI | `src/frontend/projects/admin-mfe/.../features/` | CRM-035 |
| Nav | shell + admin home links | CRM-035 |
| tsconfig | admin-mfe `tsconfig.app.json` include app sources | CRM-035 |
