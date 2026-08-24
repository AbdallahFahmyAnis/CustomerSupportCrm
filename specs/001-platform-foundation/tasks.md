# Tasks: Platform foundation

**Input**: `specs/001-platform-foundation/`

## Format

- `[P]` = parallel-safe (different files)
- `[USn]` = user story from spec.md

## Phase 1: Foundation

- [x] T001 [P] Solution, Directory.Build.props, BuildingBlocks, Contracts
- [x] T002 [P] Identity, Tickets health APIs
- [x] T003 Customers `GetBootstrapStatus` VSA query + tests
- [x] T004 Gateway YARP, BFF cookie login, aggregated `/health`, MFE + API routes

## Phase 2: Node and UI

- [x] T005 [P] NestJS Channels + Notifications `/health`
- [x] T006 Angular workspace, Native Federation, AR/EN shell, remotes
- [x] T007 [US1] Agent remote loads at `/agent`; stub remotes at `/portal`, `/admin`, `/knowledge`

## Phase N: Ship

- [x] T008 `scripts/dev.ps1` and README run instructions
- [x] T009 Smoke `http://localhost:5000/health` and shell language toggle
- [ ] T010 Stage related files only; commit when the user asks
