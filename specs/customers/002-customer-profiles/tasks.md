# Tasks: Customer profiles

**Input**: `specs/customers/002-customer-profiles`

## Format

- `[P]` = parallel-safe (different files)
- `[USn]` = user story from spec.md

## Phase 1: Foundation

- [x] T001 Domain aggregate + contact/note/attachment entities
- [x] T002 SQLite schema ensure + Contracts DTOs
- [x] T003 Wire DI / MediatR / Program endpoints

## Phase 2: CRM-001 Profiles

- [x] T004 [US1] CreateCustomer + UpdateCustomer + SearchCustomers + GetCustomer
- [x] T005 [US1] Tests `[Trait("Story", "CRM-001")]`
- [x] T006 [US1] agent-mfe list / new / detail / edit

## Phase 3: CRM-002 Contacts

- [x] T007 [US2] AddContact + DeactivateContact
- [x] T008 [US2] Tests + detail UI contacts panel

## Phase 4: CRM-003 Notes / attachments / timeline

- [x] T009 [US3] AddNote + AddAttachment + GetAttachment + GetCustomerTimeline
- [x] T010 [US3] Tests + detail UI timeline / upload / download

## Phase N: Ship

- [x] T011 Seed demo customers (must not fail startup)
- [x] T012 Smoke at `http://localhost:5000/agent/customers`
- [x] T013 Mark spec Implemented; update `specs/000-product/spec.md`
- [ ] T014 Stage related files only; commit when the user asks
