# Implementation Plan: Richer channel providers

**Spec**: `specs/channels/016-channel-providers/spec.md`  
**Story**: CRM-040  
**Workflow stage after this plan**: Apply â†’ Test â†’ Mock â†’ Done

## Summary

Add SendGrid email and Twilio SMS/WhatsApp outbound adapters behind env-gated factories; keep Dev providers as default.

## Technical Context

**Language/Version**: NestJS  
**Edge**: `http://localhost:5000`  
**Owning service**: Channels  
**Owning MFE**: none (agent reply UI unchanged)

## Constitution Check

- [x] Spec exists with screens + AC
- [x] One vertical slice
- [x] No new public port
- [x] Story id in new types/endpoints
- [x] Command/query in feature folders (providers under infrastructure)

## Code to apply *(mandatory)*

| Area | Path | Story |
|---|---|---|
| Config | `app/config.ts` SendGrid + Twilio env | CRM-040 |
| Email | `infrastructure/email/sendgrid-email.provider.ts` | CRM-040 |
| SMS | `infrastructure/sms/twilio-sms.provider.ts` | CRM-040 |
| WhatsApp | `infrastructure/whatsapp/twilio-whatsapp.provider.ts` | CRM-040 |
| DI | `app.module.ts` factories | CRM-040 |
| Tests | `test/providers-selection.test.cjs` | CRM-040 |
| Docs | README + product Next | CRM-040 |
