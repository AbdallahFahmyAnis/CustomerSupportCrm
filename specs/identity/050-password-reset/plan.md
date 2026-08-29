# Implementation Plan: Forgot password / password reset

**Spec**: `specs/identity/050-password-reset/spec.md`  
**Story**: CRM-046  
**Workflow stage after this plan**: Apply → Test → Mock → Done

## Summary

Anonymous forgot/reset using ASP.NET Identity password-reset tokens. Generic success (no enumeration). Dev/local responses may include `devResetToken` when `Identity:ExposeResetToken` is true (default in Development). Optional Channels transactional email when configured. Shell screens `/forgot-password` and `/reset-password`.

## Technical Context

**Owning service**: Identity  
**Owning MFE**: shell  
**Edge**: `http://localhost:5000`

## Constitution Check

- [x] Spec + AC
- [x] No new public port
- [x] VSA feature folders

## Code to apply

| Area | Path | Story |
|---|---|---|
| API | `Features/Auth/ForgotPassword/*`, `ResetPassword/*` | CRM-046 |
| Directory | password reset helpers on `IdentityDirectory` | CRM-046 |
| Channels | optional `POST /api/channels/mail/send` | CRM-046 |
| UI | shell forgot/reset pages (already scaffolded with register) | CRM-046 |
| Tests | CRM-046 traits | CRM-046 |
