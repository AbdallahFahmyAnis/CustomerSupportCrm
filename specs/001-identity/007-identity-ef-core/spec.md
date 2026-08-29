# Feature Specification: IdentityServer-style auth + EF Core (SQL Server & PostgreSQL)

**Story**: CRM-035 (users/roles), CRM-037 (system persistence)  
**Epic**: Security and Administration / Platform  
**Priority**: Must  
**Status**: Implemented  
**Created**: 2026-08-24

## User story

**As a** platform engineer,  
**I want** Identity backed by an IdentityServer-style stack and EF Core, using SQL Server and PostgreSQL,  
**so that** user/token storage and later Nest/.NET services use a standard ORM instead of hand-written ADO/SQLite.

## Business value

Standard identity and ORM reduce custom auth bugs, unlock migrations/tooling, and align with the Docker data platform (006).

## Scope

**In scope (proposed)**
- Replace Identity’s hand-written store with **ASP.NET Core Identity + EF Core** on **SQL Server**
- Add an **OIDC/OAuth2 authorization server** in the Identity service (**NEEDS CLARIFICATION** — product below)
- Keep gateway as the only browser edge; BFF cookie login continues to work with demo users
- Introduce **EF Core + Npgsql** for **Channels** persistence on **PostgreSQL** (migrate off JSON/file store for intake/portal data in this or immediately stacked slice)
- EF migrations checked in; `EnsureCreated` only for ephemeral tests if needed
- Constitution note: EF Core is the preferred persistence for .NET services on SQL Server / Postgres

**Out of scope**
- Duende commercial features / paid license procurement (unless chosen)
- Mongo / Knowledge EF mapping
- Customers/Tickets EF migration (follow-up; same SQL Server pattern)
- Social login, external IdP federation
- Changing demo password

## NEEDS CLARIFICATION

**Resolved — Option A:** OpenIddict + ASP.NET Core Identity + EF Core (SQL Server for Identity). Nest Channels uses **TypeORM** on PostgreSQL (EF Core is .NET-only; same ORM goal for Channels).

Default if unanswered: **A**.

## Preconditions

- `006-data-platform` Docker SQL Server + Postgres available (or connection strings set)
- Demo: `agent@crm.local` / `Crm!123` at `http://localhost:5000`

## Screens

| Screen | URL | Actor | Observable result |
|---|---|---|---|
| Shell sign-in | `http://localhost:5000/` | agent | Login still works; session cookie issued |
| Admin users | `http://localhost:5000/admin` | admin | User/role admin still works against EF Identity store |

## Acceptance criteria

1. **Given** SQL Server is up, **when** Identity starts, **then** EF Core applies Identity migrations and seeds demo users/roles.
2. **Given** the chosen auth server (A/B/C), **when** gateway `/login` succeeds, **then** access/refresh behaviour remains usable by the shell (cookies).
3. **Given** Postgres is up, **when** Channels starts with EF, **then** intake/portal tables exist via migrations and existing portal submit/track still works.
4. **Given** unit tests, **when** they run without Docker, **then** they use EF InMemory or LocalDB/Sqlite provider as documented — not a broken CI.
5. **Given** constitution, **when** this ships, **then** EF Core + SQL Server/Postgres are named as the .NET/Channels persistence approach.

## Definition of Done

- [x] Acceptance criteria pass on gateway UAT (Identity EF + OpenIddict token surface; Channels TypeORM ready)
- [x] Automated tests with `[Trait("Story", "CRM-035")]` / CRM-037 updated
- [x] Seeded demo users work (Identity hasher)
- [x] Spec, plan, and code cite this slice / CRM-035 / CRM-037

## Assumptions and dependencies

- Depends on: `004-identity-admin`, `005-channels-portal`, `006-data-platform`
- Assumptions: gateway BFF remains; browsers never talk to Identity ports; one Identity DbContext on SQL Server; Channels DbContext on Postgres
