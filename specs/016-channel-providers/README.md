# Channel providers (016 / CRM-040)

Canonical path: `specs/005-channels/016-channel-providers/`

Default: **Dev** providers (log only). Set env to enable real outbound HTTP adapters.

## Email selection (Twilio SendGrid)

1. `SENDGRID_API_KEY` (or `TWILIO_SENDGRID_API_KEY`) → **Twilio SendGrid**  
2. else `EMAIL_SMTP_HOST` → SMTP (existing)  
3. else DevEmailProvider  

```bash
# Twilio SendGrid — https://app.sendgrid.com → API Keys + verified sender
set SENDGRID_API_KEY=SG....
set SENDGRID_FROM=support@example.com
```

SMS/WhatsApp use `TWILIO_ACCOUNT_*`; email does **not** — it needs a SendGrid API key.

## SMS selection

1. `TWILIO_ACCOUNT_SID` + `TWILIO_AUTH_TOKEN` + `TWILIO_SMS_FROM` → Twilio SMS  
2. else DevSmsProvider  

```bash
set TWILIO_ACCOUNT_SID=AC....
set TWILIO_AUTH_TOKEN=....
set TWILIO_SMS_FROM=+15551234567
```

## WhatsApp selection

1. same Twilio account vars + `TWILIO_WHATSAPP_FROM` (e.g. `whatsapp:+14155238886`) → Twilio WhatsApp  
2. else DevWhatsAppProvider  

```bash
set TWILIO_WHATSAPP_FROM=whatsapp:+14155238886
```

## Reply (unchanged)

Use agent ticket detail or existing curl under `013` / `015` / email outbound README. When a configured provider fails, the API returns an error and does not silently fall back.

## Inbound webhooks (CRM-040 / 024)

Twilio posts form-urlencoded callbacks. Signature uses `TWILIO_AUTH_TOKEN` when set; empty token = **dev bypass**.

| Channel | Gateway URL |
|---|---|
| SMS | `POST http://localhost:5000/api/channels/webhooks/twilio/sms` |
| WhatsApp | `POST http://localhost:5000/api/channels/webhooks/twilio/whatsapp` |

```bash
set CHANNELS_PUBLIC_URL=http://localhost:5000
set TWILIO_AUTH_TOKEN=....
```

`CHANNELS_PUBLIC_URL` must match the URL Twilio is configured to call (used when validating `X-Twilio-Signature`).

JSON intake `/api/channels/intake/sms` and `/whatsapp` remains for local curls without signatures.
