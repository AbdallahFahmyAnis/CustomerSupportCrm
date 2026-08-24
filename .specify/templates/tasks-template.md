# Tasks: [FEATURE NAME]

**Input**: `specs/[###-feature-name]/`

## Format

- `[P]` = parallel-safe (different files)
- `[USn]` = user story from spec.md

## Phase 1: Foundation

- [ ] T001 Schema/entity/DTOs in the owning service
- [ ] T002 Map endpoints on the existing API group (feature folder, not a new layer)

## Phase 2: User Story 1

- [ ] T003 [US1] API command or query
- [ ] T004 [US1] Angular surface in the owning MFE
- [ ] T005 [US1] Seed demo data (must not fail startup)

## Phase N: Ship

- [ ] T00N Restart the touched service and smoke the success criteria at `http://localhost:5000`
- [ ] T00N+1 Stage related files only; commit when the user asks
