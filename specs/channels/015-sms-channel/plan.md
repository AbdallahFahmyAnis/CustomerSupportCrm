# Implementation Plan: SMS channel

**Spec**: `specs/channels/015-sms-channel/spec.md`  
**Story**: CRM-011 / CRM-040  
**Workflow stage after this plan**: Apply â†’ Test â†’ Mock â†’ Done

## Summary

Mirror WhatsApp intake/reply with SMS: Nest features, DevSmsProvider, widen channel union to `Sms`, agent reply channel option.

## Technical Context

**Language/Version**: NestJS / Angular  
**Edge**: `http://localhost:5000`  
**Owning service**: Channels  
**Owning MFE**: agent-mfe

## Constitution Check

- [x] Spec exists with screens + AC
- [x] One vertical slice
- [x] No new public port
- [x] Story id in new types/endpoints
- [x] Command/query in feature folders

## Code to apply *(mandatory)*

| Area | Path | Story |
|---|---|---|
| Domain | `channels.models.ts` add `Sms` | CRM-011 |
| Provider | `infrastructure/sms/*` | CRM-040 |
| Intake | `features/intake/ingest-sms/*` | CRM-011 |
| Reply | `features/messages/reply-sms/*` | CRM-011 |
| Downstream | `findOrCreateCustomerBySms`, phone resolve includes Sms | CRM-011 |
| DI | `app.module.ts` | CRM-011 |
| FE | agent-mfe tickets API/store/detail | CRM-011 |
| Tests | `test/validate-web-form.test.cjs` | CRM-011 |
| Docs | README + product Next | CRM-011 |
