# Feature Specification: Customer self-registration

**Story**: CRM-045  
**Epic**: Identity & Access (`specs/identity`)  
**Priority**: Must  
**Status**: Implemented  
**Created**: 2026-08-29

## User story

**As a** prospective customer,  
**I want** to create my own portal account with email and password,  
**so that** I can sign in, submit and track my requests without relying on a seeded demo user or email-only lookup.

## Business value

Opens the portal to real customers instead of staff-created or demo-only accounts; reduces agent time spent provisioning portal logins.

## Scope

**In scope**
- Public **Register** screen (anonymous) on the shell, linked from `/login` and portal home
- Self-service create of an Identity user with role **Customer** only (cannot choose Agent/Admin/Lead)
- Required fields: display name, email, password, password confirmation (UI)
- Reject duplicate email with a clear validation message
- After successful register: establish the same gateway BFF session as login (or equivalent immediate sign-in) and land on `/portal`
- Ensure a **Customers** profile exists for that email (`UniqueIdentifier` = email) so submit/track and agent CRM stay linked
- Audit-friendly Identity write (reuse existing create-user / directory patterns; public endpoint must not expose staff roles)
- Seed remains: `customer@crm.local` / `Crm!123` still works; registration is additive
- Bilingual (EN/AR) labels on the register form

**Out of scope**
- Email verification / magic-link confirmation (follow-up; account is usable immediately after register)
- Password reset / forgot-password â†’ **CRM-046** / `specs/identity/050-password-reset`
- Social login / SSO / MFA
- Admin “invite customer” flow (admin can still create Customer users via CRM-035)
- Changing portal track to *require* login (anonymous email track from CRM-028 stays)
- Captcha / rate-limit productization beyond existing Identity lockout for *login*

## Preconditions

- `004-identity-admin`, `007-identity-ef-core`, `005-channels-portal`, `002-customer-profiles` available
- Gateway proxies Identity and Customers; browsers use `http://localhost:5000` only
- Customer role already seeded; demo `customer@crm.local` can sign in today

## Screens

| Screen | URL | Actor | Observable result |
|---|---|---|---|
| Register | `http://localhost:5000/register` | anonymous | Form: name, email, password, confirm; success â†’ signed in at `/portal` |
| Login | `http://localhost:5000/login` | anonymous | Link “Create account” / Register |
| Portal home | `http://localhost:5000/portal` | anonymous or customer | Link to Register when signed out |
| Track (smoke) | `http://localhost:5000/portal/track` | newly registered customer | Signed-in email prefilled / auto-track works |

## Acceptance criteria

1. **Given** I am signed out, **when** I open Register and submit a valid name, new email, and matching passwords, **then** an Identity user with role Customer is created and I am signed in at the portal.
2. **Given** a successful register, **when** an agent searches Customers by that email, **then** a customer profile exists (created or already linked) with that email as unique identifier.
3. **Given** an email that already has an Identity account, **when** I register again, **then** the API rejects with a clear duplicate/conflict message and no second user is created.
4. **Given** mismatched passwords or missing required fields, **when** I submit, **then** the UI/API rejects without creating a user.
5. **Given** the public register API, **when** a caller tries to register with role Agent/Admin/Lead (or omits forced Customer), **then** the account is still only Customer (role is not attacker-controlled).
6. **Given** I just registered, **when** I open Track, **then** my session email is used (same behaviour as seeded customer login).
7. **Given** the gateway is up, **when** the browser registers, **then** traffic goes through `/api/...` on `:5000` only (no direct Identity/Customers ports).

## Definition of Done

- [x] Acceptance criteria pass in UAT on the gateway
- [x] Automated test `[Trait("Story", "CRM-045")]` (Identity register + role force Customer)
- [x] Mock/seed: existing `customer@crm.local` still login; register path clickable from login
- [x] Spec, plan, and code cite `CRM-045`

## Assumptions and dependencies

- Depends on: CRM-027 / CRM-028 (portal), CRM-035 (Identity users/roles), CRM-001 (customer profiles)
- Assumptions:
  - No email verification in this slice (SendGrid verification of *sender* is unrelated)
  - Minimum password rules match ASP.NET Identity defaults already used for CreateUser
  - Register is anonymous (no BFF cookie required); CreateUser admin endpoint stays Admin-only
  - Linking Customers profile uses email as `UniqueIdentifier` (same convention as portal intake)
- Demo password docs remain `Crm!123` for seeded users only; self-registered users choose their own password
