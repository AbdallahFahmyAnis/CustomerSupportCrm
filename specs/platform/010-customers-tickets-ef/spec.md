# Feature Specification: Customers & Tickets EF Core

**Story**: CRM-001…007 (persistence), CRM-037 (platform)  
**Epic**: Platform / Customer & Ticket Management  
**Priority**: Must  
**Status**: Implemented  
**Created**: 2026-08-25

## User story

**As a** platform engineer,  
**I want** Customers and Tickets to persist via EF Core on SQL Server (Sqlite offline),  
**so that** .NET services share the same ORM approach as Identity and use the Docker SQL Server from the data platform.

## Business value

Removes hand-written ADO/Sqlite drift; enables migrations and aligns with constitution EF Core guidance.

## Scope

**In scope**
- Replace `CustomersDb` / `TicketsDb` ADO Sqlite with **EF Core** DbContexts
- SQL Server when `ConnectionStrings:Customers` / `ConnectionStrings:Tickets` (or Provider=SqlServer); Sqlite file escape hatch for tests (`*:DataPath`)
- Keep existing HTTP contracts and CQRS handlers behaviour
- Keep customer **attachments on disk** (path under data root)
- Update tests factories; ensure existing Customer/Tickets tests pass
- Spec docs + product backlog update

**Out of scope**
- Identity changes
- Channels Nest persistence
- Fluent migrations checked into CI as separate deploy step (EnsureCreated acceptable for this slice)
- CRM-036 audit UI

## Screens

| Screen | URL | Actor | Observable result |
|---|---|---|---|
| Customer list/detail | agent customers | agent | CRUD still works |
| Ticket list/detail | agent tickets | agent | Lifecycle still works |

## Acceptance criteria

1. **Given** Sqlite DataPath (tests), **when** Customers/Tickets APIs run, **then** existing automated tests pass.
2. **Given** SQL Server connection strings, **when** services start, **then** EF creates schema and seed/demo flows still work.
3. **Given** gateway UI paths for customers/tickets, **when** agent uses them, **then** behaviour matches pre-migration contracts.
4. **Given** an attachment upload, **when** stored, **then** file remains on disk under the configured data root.

## Definition of Done

- [x] Customer + Ticket tests green
- [x] Spec/plan/code cite `010-customers-tickets-ef` / CRM-037
- [x] Product Next updated

## Assumptions and dependencies

- Depends on: `006-data-platform`, `007-identity-ef-core`, shipped customer/ticket features
- Assumptions: Guid keys as strings/uniqueidentifiers mapped consistently with existing APIs
