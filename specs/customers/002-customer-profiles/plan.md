# Implementation Plan: Customer profiles

**Spec**: `specs/customers/002-customer-profiles/spec.md`  
**Story**: CRM-001 / CRM-002 / CRM-003  
**Workflow stage after this plan**: Apply â†’ Test â†’ Mock â†’ Done

## Summary

Extend Customers (.NET) with SQLite persistence and vertical-slice CQRS for profiles, contacts, notes, and attachments. Expose APIs through the existing gateway `/api/customers/*` routes. Replace the agent-mfe bootstrap placeholder with customer list/detail/create/edit screens that call only `/api/...`.

## Technical Context

**Language/Version**: .NET 9 / Angular 19  
**Edge**: `http://localhost:5000`  
**Owning service**: Customers (`Crm.Customers.Api` :5102)  
**Owning MFE**: agent-mfe

## Constitution Check

- [x] Spec exists with screens + AC
- [x] One vertical slice (Customers + agent-mfe only)
- [x] No new public port
- [x] Story id in new types/endpoints
- [x] Command/query live in a feature folder (VSA + CQRS)

## Persistence

- SQLite file `customers.db` under service content root `data/`
- Tables: `Customers`, `Contacts`, `Notes`, `Attachments` (Guid TEXT ids)
- Attachments binary on disk: `data/attachments/{customerId}/{attachmentId}_{fileName}`
- Seed wrapped in try/catch so startup never dies

## Features (VSA)

| Feature | Type | Story |
|---|---|---|
| `CreateCustomer` | Command | CRM-001 |
| `UpdateCustomer` | Command | CRM-001 |
| `SearchCustomers` | Query | CRM-001 |
| `GetCustomer` | Query | CRM-001 |
| `AddContact` | Command | CRM-002 |
| `DeactivateContact` | Command | CRM-002 |
| `AddNote` | Command | CRM-003 |
| `AddAttachment` | Command | CRM-003 |
| `GetAttachment` | Query | CRM-003 |
| `GetCustomerTimeline` | Query | CRM-003 |

## Code to apply *(mandatory)*

| Area | Path | Story |
|---|---|---|
| Domain | `src/services/customers/Crm.Customers.Api/Domain/` | CRM-001…003 |
| Persistence | `.../Infrastructure/CustomersDb.cs`, schema ensure, seeder | CRM-001 |
| Features | `.../Features/*/` | CRM-001…003 |
| Contracts | `contracts/csharp/Crm.Contracts/Customers/` | CRM-001…003 |
| Tests | `tests/Crm.Customers.Api.Tests/` | CRM-001…003 |
| UI | `src/frontend/projects/agent-mfe/src/app/customers/` | CRM-001…003 |
| Routes | `.../agent-mfe/.../remote.routes.ts` | CRM-001 |
| Shell nav | optional link label already “Agent”; customers under `/agent/customers` | CRM-001 |
