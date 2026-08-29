# Implementation Plan: System configuration

**Spec**: `specs/001-identity/012-system-config/spec.md`  
**Story**: CRM-037  
**Workflow stage after this plan**: Apply â†’ Test â†’ Mock â†’ Done

## Summary

Add Identity EF `SystemSettings` (singleton row), admin GET/PUT APIs, apply lockout numbers from settings on failed login, and an admin-mfe Settings page.

## Technical Context

**Language/Version**: .NET 9 / Angular  
**Edge**: `http://localhost:5000`  
**Owning service**: Identity  
**Owning MFE**: admin-mfe

## Constitution Check

- [x] Spec exists with screens + AC
- [x] One vertical slice (Identity + admin-mfe)
- [x] No new public port
- [x] Story id in new types/endpoints
- [x] Command/query in feature folders (VSA + CQRS)

## Code to apply *(mandatory)*

| Area | Path | Story |
|---|---|---|
| Domain/EF | `Domain/SystemSettings.cs`, DbContext, EnsureSchema create-if-missing, seeder | CRM-037 |
| API | `Features/Settings/GetSettings/*`, `UpdateSettings/*` | CRM-037 |
| Apply | `IdentityDirectory.RegisterFailedLoginAsync` reads settings for lockout | CRM-037 |
| Contract | `SystemSettingsDto`, `UpdateSystemSettingsRequest` | CRM-037 |
| FE | `admin-mfe/features/settings/*` + home + remote route | CRM-037 |
| Tests | `IdentityAdminTests` CRM-037 | CRM-037 |
| Docs | product Next, feature.json | CRM-037 |
