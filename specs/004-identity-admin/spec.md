# Feature Specification: Identity admin — users, roles, permissions

**Story**: CRM-035  
**Epic**: Security and Administration  
**Priority**: Must  
**Status**: Implemented  
**Created**: 2026-08-24

## User story

**As an** administrator,  
**I want** to manage users, assign roles, and see which permissions each role grants,  
**so that** only the right people can work tickets and administer the CRM.

## Business value

Access control becomes explicit and auditable instead of a single hardcoded demo login.

## Scope

**In scope**
- CRM-035: list/create/deactivate users; assign one primary role; list roles with permission sets; permission catalog
- Sign-in validates against persisted Identity users (seed keeps `agent@crm.local` / `Crm!123`)
- Seed an Admin user for admin-mfe work
- admin-mfe Feature-Based + Signals under `/admin/users` and `/admin/roles`
- Access JWT + refresh token; refresh rotation; revoke refresh/access (jti blacklist)
- Max failed login attempts with temporary lockout
- Gateway BFF keeps tokens server-side (httpOnly refresh cookie); browser never stores access token

**Out of scope**
- CRM-036 audit logs
- CRM-037 system configuration
- Full OIDC / Entra ID
- Fine-grained per-endpoint enforcement on Customers/Tickets (headers already carry role; deny paths later)
- Password reset / MFA

## Preconditions

- `001-platform-foundation` available; gateway proxies `/api/identity/*`
- Caller signed in for admin screens; Admin role required to mutate users/roles

## Screens

| Screen | URL | Actor | Observable result |
|---|---|---|---|
| Users | `http://localhost:5000/admin/users` | Admin | List users; create; deactivate; change role |
| Roles | `http://localhost:5000/admin/roles` | Admin | Roles with permission chips |
| Sign-in | `http://localhost:5000/` | any | Seeded agent and admin can sign in |

## Acceptance criteria

1. **Given** I am Admin, **when** I open Users, **then** I see seeded users including Demo Agent and Admin.
2. **Given** required fields, **when** I create a user with a role, **then** they appear in the list and can sign in with the password I set.
3. **Given** an active non-self user, **when** I deactivate them, **then** they cannot sign in and show as Inactive.
4. **Given** a user, **when** I change their role, **then** the new role is saved and returned on next login/session.
5. **Given** I open Roles, **when** the page loads, **then** I see Admin / Agent / Lead (or equivalent) with their permissions.
6. **Given** I am not Admin, **when** I call user mutation APIs, **then** the system blocks the change.
7. **Given** valid credentials, **when** I sign in via the gateway, **then** Identity issues an access token and refresh token (gateway keeps them; UI still uses BFF cookie).
8. **Given** a valid refresh token, **when** I refresh, **then** I receive a new access + refresh pair and the old refresh is revoked.
9. **Given** a revoked refresh token, **when** I refresh or reuse it, **then** Identity rejects it.
10. **Given** too many failed logins (default 5), **when** I try again, **then** the account is locked for a cooldown window and sign-in is blocked.

## Definition of Done

- [x] AC pass on gateway UAT
- [x] Tests `[Trait("Story", "CRM-035")]`
- [x] Seeded users clickable without manual setup
- [x] Spec/plan/code cite CRM-035; FE Feature-Based + Signals with html/scss/ts
- [x] Token/refresh/revoke + lockout tests pass

## Assumptions and dependencies

- Depends on: `001-platform-foundation`
- Assumptions: SQLite in Identity; single primary role per user; passwords stored with a simple hash suitable for local demo (not production KMS); JWT signing key from config for local demo
- Demo passwords remain `Crm!123` in seed docs
- Defaults: max 5 failed attempts, 15-minute lockout, ~15-minute access token, ~7-day refresh token
