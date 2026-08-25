# Feature Specification: System configuration

**Story**: CRM-037  
**Epic**: Security and Administration  
**Priority**: Must  
**Status**: Implemented  
**Created**: 2026-08-25

## User story

**As an** administrator,  
**I want** to view and update system settings (organization identity and login lockout policy),  
**so that** I can tune the CRM without redeploying code.

## Business value

Closes the remaining CRM-037 admin gap after platform persistence slices — operators can configure tenant display and security lockout from the UI.

## Scope

**In scope**
- Persist a single system-settings document in Identity (EF): organization name, support email, default culture (`en` | `ar`), max failed logins, lockout minutes
- `GET` / `PUT` admin settings API
- Lockout policy from settings applied on failed login (not only compile-time constants)
- admin-mfe **Settings** page (Feature-Based + Signals) with save
- Seed defaults matching current demo behaviour (5 attempts / 15 minutes; org “Customer Support CRM”)
- Record `SettingsUpdated` in audit log when saved
- Automated Identity tests for get/put + non-admin 403

**Out of scope**
- Editing JWT signing keys / connection strings from the UI
- Full SLA policy editor (CRM-017+)
- Per-channel provider secrets (SendGrid, etc.)
- Multi-tenant orgs

## Preconditions

- `004-identity-admin` and `011-audit-logs` available
- Caller uses Admin role via gateway headers/BFF

## Screens

| Screen | URL | Actor | Observable result |
|---|---|---|---|
| Settings | `http://localhost:5000/admin/settings` | admin | Form with org + lockout fields; Save persists and reloads |

## Acceptance criteria

1. **Given** an admin session, **when** I open Settings, **then** I see seeded defaults (org name, support email, culture, lockout policy).
2. **Given** valid edits, **when** I save, **then** GET returns the new values and an audit `SettingsUpdated` entry exists.
3. **Given** I lower max failed attempts to 2, **when** a user fails password twice, **then** the account locks for the configured lockout minutes.
4. **Given** a non-admin role, **when** I GET or PUT settings, **then** I receive 403.
5. **Given** invalid values (empty org name, max attempts &lt; 1, lockout minutes &lt; 1, culture not en/ar), **when** I save, **then** the API rejects with 400.

## Definition of Done

- [x] AC pass on gateway admin UI
- [x] Automated test `[Trait("Story", "CRM-037")]`
- [x] Spec/plan/code cite CRM-037
- [x] Product backlog updated (CRM-037 remaining UI done)

## Assumptions and dependencies

- Depends on: `004-identity-admin`, `011-audit-logs`
- Assumptions: single global settings row; Identity remains the owner for admin security/config settings
