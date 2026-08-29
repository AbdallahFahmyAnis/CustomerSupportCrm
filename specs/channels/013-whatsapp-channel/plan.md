# Implementation Plan: WhatsApp channel

**Spec**: `specs/channels/013-whatsapp-channel/spec.md`  
**Story**: CRM-009 / CRM-040  
**Workflow stage after this plan**: Apply â†’ Test â†’ Mock â†’ Done

## Summary

Mirror email intake/reply with WhatsApp: Nest features, DevWhatsAppProvider, widen channel union, agent reply form.

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
| Domain | `channels.models.ts` add `WhatsApp` | CRM-009 |
| Provider | `infrastructure/whatsapp/*` | CRM-040 |
| Intake | `features/intake/ingest-whatsapp/*` | CRM-009 |
| Reply | `features/messages/reply-whatsapp/*` | CRM-009 |
| Downstream | `findOrCreateCustomerByPhone`, `getCustomerPhone` | CRM-009 |
| DI | `app.module.ts` | CRM-009 |
| FE | agent-mfe tickets API/store/detail | CRM-009 |
| Tests | `test/validate-web-form.test.cjs` | CRM-009 |
| Docs | README + product Next | CRM-009 |
