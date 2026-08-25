# Implementation Plan: Customers & Tickets EF Core

**Spec**: `specs/010-customers-tickets-ef/spec.md`  
**Story**: CRM-037  

## Summary

Replace ADO Sqlite in Customers/Tickets with EF Core DbContexts; keep sync `CustomersDb`/`TicketsDb` facades; Scoped DbContext via `IDbContextFactory` or scoped facade; SQL Server when connection string set else Sqlite DataPath.

## Code to apply

| Area | Path |
|---|---|
| Customers | `Infrastructure/Persistence/CustomersDbContext.cs`, rewrite `CustomersDb.cs`, DI, csproj |
| Tickets | `Infrastructure/Persistence/TicketsDbContext.cs`, rewrite `TicketsDb.cs`, DI, csproj |
| Tests | factories force Sqlite Provider |
| Docs | README, product, constitution note if needed |

## Constitution Check

- [x] One platform persistence slice; no new port
