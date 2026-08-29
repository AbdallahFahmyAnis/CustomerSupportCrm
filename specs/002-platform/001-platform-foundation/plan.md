# Implementation Plan: Platform foundation

**Spec**: `specs/002-platform/001-platform-foundation/spec.md`  
**Story**: CRM-041 / CRM-042 (chrome) + platform 001  
**Workflow stage after this plan**: Apply â†’ Test â†’ Mock â†’ Done

## Summary

Stand up the polyglot edge: YARP BFF gateway, Identity/Customers/Tickets .NET APIs (Customers includes a MediatR `GetBootstrapStatus` slice), NestJS Channels and Notifications health, and an Angular Native Federation workspace whose remotes are reached only through the gateway.

## Technical Context

**Language/Version**: .NET 9 / NestJS 10+ / Angular 19  
**Edge**: `http://localhost:5000`  
**Owning service**: Gateway (+ Identity, Customers, Tickets, Channels, Notifications)  
**Owning MFE**: shell + agent-mfe (010-portal/admin/knowledge stubs)

## Constitution Check

- [x] Spec exists with screens + AC
- [x] One vertical slice (platform skeleton; no customer CRUD)
- [x] No new public port (5000 only)
- [x] Story id in new types/endpoints
- [x] Command/query live in a feature folder (VSA + CQRS)

## Code to apply *(mandatory)*

| Area | Path | Story |
|---|---|---|
| Gateway | `src/gateway/Crm.Gateway/` | CRM-041 |
| BuildingBlocks | `src/building-blocks/Crm.BuildingBlocks/` | 001 |
| Contracts | `contracts/csharp/Crm.Contracts/` | 001 |
| Identity | `src/services/identity/Crm.Identity.Api/` | 001 |
| Customers | `src/services/customers/Crm.Customers.Api/Features/GetBootstrapStatus/` | 001 |
| Tickets | `src/services/tickets/Crm.Tickets.Api/` | 001 |
| Tests | `tests/Crm.Customers.Api.Tests/` | `[Trait("Story", "CRM-041")]` |
| Channels | `src/services/channels/` | 001 |
| Notifications | `src/services/notifications/` | 001 |
| Frontend | `src/frontend/projects/{shell,agent-mfe,portal-mfe,admin-mfe,knowledge-mfe,shared}` | CRM-041, CRM-042 |
| Run | `scripts/dev.ps1`, `README.md` | 001 |
