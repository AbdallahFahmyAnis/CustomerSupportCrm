# Inbound webhooks (024 / CRM-040)

Canonical path: `specs/005-channels/024-inbound-webhooks/`

Twilio SMS / WhatsApp form webhooks with optional `X-Twilio-Signature` validation.

## Endpoints (via gateway)

| Method | Path |
|---|---|
| POST | `/api/channels/webhooks/twilio/sms` |
| POST | `/api/channels/webhooks/twilio/whatsapp` |

## Env

| Var | Purpose |
|---|---|
| `CHANNELS_PUBLIC_URL` | Base URL Twilio calls (default `http://localhost:5000`) |
| `TWILIO_AUTH_TOKEN` | When set, signatures required; when empty, validation is bypassed for local demos |

## Local smoke (dev bypass)

```powershell
curl -X POST http://localhost:5000/api/channels/webhooks/twilio/sms `
  -H "Content-Type: application/x-www-form-urlencoded" `
  -d "From=%2B15551234567&Body=Hello%20from%20Twilio"
```

JSON intake `/api/channels/intake/sms` remains for unsigned local curls.
