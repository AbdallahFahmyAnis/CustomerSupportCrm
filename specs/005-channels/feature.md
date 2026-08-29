# Feature: Channels

**Slug**: `005-channels`  
**Status**: Active (multi-slice)  
**Owning service**: Channels (+ Tickets / portal-mfe)  

## Stories

| Story | Slice | Status | Summary |
|---|---|---|---|
| CRM-012 / 027 / 028 | [005-channels-portal](005-channels-portal/spec.md) | Implemented | Web form + portal submit/track |
| CRM-008 | [008-email-channel](008-email-channel/spec.md) | Implemented | Email intake |
| CRM-040 | [009-email-outbound](009-email-outbound/spec.md) | Implemented | Outbound email reply |
| CRM-009 | [013-whatsapp-channel](013-whatsapp-channel/spec.md) | Implemented | WhatsApp intake + reply |
| CRM-010 | [014-live-chat](014-live-chat/spec.md) | Implemented | Live chat + portal |
| CRM-011 | [015-sms-channel](015-sms-channel/spec.md) | Implemented | SMS intake + reply |
| CRM-040 | [016-channel-providers](016-channel-providers/spec.md) | Implemented | SendGrid + Twilio adapters |
| CRM-040 | [024-inbound-webhooks](024-inbound-webhooks/spec.md) | Implemented | Twilio inbound signature verify |

## Ops docs

- [016-channel-providers/README.md](016-channel-providers/README.md) — SendGrid / Twilio env matrix
- [024-inbound-webhooks/README.md](024-inbound-webhooks/README.md) — inbound webhook paths + smoke

## Related

- Portal FAQs: `008-knowledge/022-portal-faqs`
- Password reset email: `001-identity/050-password-reset`
