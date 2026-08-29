# Feature Specification: Multi-database Docker platform

**Story**: CRM-037 (system configuration / platform persistence); slice `006`  
**Epic**: Security and Administration / Platform  
**Priority**: Must  
**Status**: Implemented  
**Created**: 2026-08-24

## User story

**As a** developer or CI agent,  
**I want** SQL Server, PostgreSQL, and MongoDB available via Docker and an Azure Pipeline that builds and tests against them,  
**so that** services can move off local SQLite with a repeatable local and CI environment.

## Business value

Removes “works on my machine” persistence drift and prepares owned services for production-grade engines without inventing a new public edge.

## Scope

**In scope**
- `docker-compose.yml` with SQL Server, PostgreSQL, and MongoDB (dev credentials documented; not for production)
- Azure Pipelines YAML: restore/build, service containers for the three DBs, run automated tests
- Identity service persistence on **SQL Server** when `ConnectionStrings:Identity` is set (proof path for .NET)
- Documented engine mapping for later slices: SQL Server → Customers/Tickets; Postgres → Channels; Mongo → Knowledge (container only this slice)
- Constitution amendment allowing these containers and engines when named by this spec
- README / compose usage: `docker compose up -d` before Identity when using SQL Server

**Out of scope**
- Migrating Customers, Tickets, or Channels off SQLite/JSON (follow-up slices)
- Wiring Knowledge to Mongo
- Production cloud PaaS provisioning, secrets rotation, backups
- Message brokers
- Changing the gateway public edge or demo password

## Preconditions

- Docker Desktop (or compatible engine) available locally for compose.
- Azure DevOps pipeline can pull Microsoft/Postgres/Mongo images.
- Seeded demo users still `agent@crm.local` / `Crm!123` when Identity starts against SQL Server.

## Screens

| Screen | URL | Actor | Observable result |
|---|---|---|---|
| Shell sign-in | `http://localhost:5000/` | agent | After compose + Identity on SQL Server, demo login still works |
| (CI) Pipeline run | Azure DevOps Pipelines | engineer | Green build/test against service containers |

## Acceptance criteria

1. **Given** Docker is available, **when** I run `docker compose up -d`, **then** SQL Server, Postgres, and Mongo containers start and expose documented ports.
2. **Given** compose is up and Identity is configured with `ConnectionStrings:Identity`, **when** Identity starts, **then** it creates schema/seeds on SQL Server and `/health` succeeds.
3. **Given** Identity uses SQL Server, **when** I sign in through the gateway with `agent@crm.local` / `Crm!123`, **then** I receive a session as today.
4. **Given** no SQL connection string (tests / no-Docker escape hatch), **when** Identity runs with `Identity:DataPath`, **then** SQLite still works so existing unit tests stay offline-capable.
5. **Given** the Azure Pipeline runs, **when** the job completes, **then** .NET restore/build/test run with SQL Server (and Postgres/Mongo containers healthy for later use).
6. **Given** the constitution, **when** this slice ships, **then** it names SQL Server / Postgres / Mongo + Docker compose + Azure Pipelines as allowed platform dependencies.

## Definition of Done

- [x] Acceptance criteria pass (compose up + Identity login + pipeline YAML present)
- [x] Automated Identity tests still pass (`[Trait("Story", "CRM-035")]`; CRM-037 on login smoke)
- [x] Mock/seed demo users work on SQL Server (EnsureSchema + SeedIfEmpty)
- [x] Spec, plan, and code cite `006-data-platform` / CRM-037

## Assumptions and dependencies

- Depends on: `001-platform-foundation`, `004-identity-admin`
- Assumptions: default compose passwords are local-dev only; Mongo/Postgres are reserved for next service migrations; one vertical “platform” slice is acceptable for infra
