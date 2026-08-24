# Implementation Plan: [FEATURE]

**Spec**: `specs/NNN-slug/spec.md`  
**Story**: CRM-nnn  
**Workflow stage after this plan**: Apply → Test → Mock → Done

## Summary

[One paragraph]

## Technical Context

**Language/Version**: .NET 9 / NestJS / Angular  
**Edge**: `http://localhost:5000`  
**Owning service**: [Customers | Tickets | Identity | Channels | …]  
**Owning MFE**: [shell | agent-mfe | portal-mfe | admin-mfe | knowledge-mfe]

## Constitution Check

- [ ] Spec exists with screens + AC
- [ ] One vertical slice
- [ ] No new public port
- [ ] Story id in new types/endpoints
- [ ] Command/query live in a feature folder (VSA + CQRS)

## Code to apply *(mandatory)*

| Area | Path | Story |
|---|---|---|
| API | `src/services/.../Features/*.cs` (or Nest `src/...`) | CRM-nnn |
| UI | `src/frontend/projects/...` | CRM-nnn |
| Tests | `tests/...` | `[Trait("Story", "CRM-nnn")]` |
| Mock | seeder / schema | CRM-nnn |
