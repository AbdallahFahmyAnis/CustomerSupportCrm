# Feature Specification: Forgot password / password reset

**Story**: CRM-046  
**Epic**: Identity & Access (`specs/identity`)  
**Priority**: Must  
**Status**: Draft  
**Created**: 2026-08-29

## User story

**As a** user who forgot my password (customer or staff),  
**I want** to request a reset link by email and set a new password,  
**so that** I can regain access without an administrator resetting my account.

## Business value

Cuts support load for locked-out customers and agents; unlocks safe self-service after CRM-045 registration.

## Scope

**In scope**
- Anonymous **Forgot password** screen: enter email → always show a generic success message (do not reveal whether the email exists)
- Identity issues a single-use, time-limited reset token for active accounts
- Outbound email with reset link via Channels/SendGrid when email is configured; otherwise a **dev-safe** path (log/token visible only in local/dev response or Channels log) so UAT works without SMTP
- Anonymous **Reset password** screen: token + new password + confirm → password updated; token invalidated
- After successful reset: redirect to login (optional auto sign-in is nice-to-have, not required)
- Links from `/login` (and register if present): “Forgot password?”
- Works for Customer, Agent, Lead, and Admin roles equally
- Bilingual EN/AR copy on both screens
- Audit-friendly Identity event on successful reset (and optionally on request) using existing audit patterns (CRM-036)

**Out of scope**
- MFA / TOTP
- Change-password while signed in (profile settings) — follow-up
- Admin force-reset from users screen (admin can still set password via create/edit if already supported)
- Account unlock email separate from reset (lockout cooldown from CRM-037 remains)
- SMS-based reset

## Preconditions

- Identity EF + sign-in (`004`, `007`) available
- Gateway BFF session login works
- Email outbound available when SendGrid/SMTP configured (`009` / `016`); local demo may use Dev email log
- Part of Identity feature map: `specs/identity/feature.md`

## Screens

| Screen | URL | Actor | Observable result |
|---|---|---|---|
| Forgot password | `http://localhost:5000/forgot-password` | anonymous | Email field; generic “if an account exists…” confirmation |
| Reset password | `http://localhost:5000/reset-password?token=…` | anonymous | New password + confirm; success → login |
| Login | `http://localhost:5000/login` | anonymous | “Forgot password?” link |

## Acceptance criteria

1. **Given** I am signed out, **when** I submit a known active account email on Forgot password, **then** I see a generic confirmation and a reset token is issued (email sent or available via documented local/dev path).
2. **Given** I submit an unknown email, **when** Forgot password completes, **then** I see the **same** generic confirmation and no user enumeration detail is returned.
3. **Given** a valid unused token within expiry, **when** I set a matching new password, **then** I can sign in with the new password and the old password fails.
4. **Given** a used or expired or forged token, **when** I submit Reset password, **then** the API rejects with a clear error and the password is unchanged.
5. **Given** mismatched or empty passwords, **when** I submit Reset, **then** validation fails without consuming a valid token (or token remains usable only if explicitly documented otherwise — prefer: do not consume on validation failure).
6. **Given** the gateway is up, **when** I use forgot/reset from the browser, **then** all traffic goes through `:5000` only.
7. **Given** a successful reset, **when** an Admin opens audit logs (if wired), **then** a success audit entry for password reset is present (or documented as deferred if audit hook is not yet on that path — prefer present).

## Definition of Done

- [ ] Acceptance criteria pass in UAT on the gateway
- [ ] Automated test `[Trait("Story", "CRM-046")]` (token issue/consume/expiry + no user enumeration)
- [ ] Mock: can reset `customer@crm.local` (or a dedicated seed) via local/dev token path without production email
- [ ] Spec, plan, and code cite `CRM-046`; listed under `specs/identity`

## Assumptions and dependencies

- Depends on: CRM-035 (Identity users), email outbound when not in local/dev mode
- Assumptions:
  - Token lifetime default ~60 minutes; single use
  - Reset link base URL uses gateway public origin (`http://localhost:5000` locally)
  - Password strength matches ASP.NET Identity rules used for CreateUser / register
  - Deactivated users do not receive a usable reset (still show generic confirmation)
- Related draft: CRM-045 register (`049`) — out of scope there now points here
