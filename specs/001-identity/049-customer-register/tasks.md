# Tasks: Customer self-registration

**Input**: `specs/001-identity/049-customer-register`

## Phase 1: Spec

- [x] T001 Write spec.md / plan.md / tasks.md

## Phase 2: Identity + Gateway

- [x] T002 `RegisterCustomer` command; Customer role only; duplicate email validation
- [x] T003 Gateway `POST /register` — BFF cookies like login
- [x] T004 Best-effort Customers profile create (`UniqueIdentifier` = email)
- [x] T005 Integration tests CRM-045

## Phase 3: shell

- [x] T006 `/register` page (EN/AR); links from login / portal
- [x] T007 Hide name/email on portal submit when signed in (follow-up UX)

## Phase N: Ship

- [x] T008 Smoke register → land on `/portal`
- [x] T009 Mark Implemented; update 000-product
