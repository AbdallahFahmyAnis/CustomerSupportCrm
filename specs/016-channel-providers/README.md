# Channel providers (016 / CRM-040)

Default: **Dev** providers (log only). Set env to enable real outbound HTTP adapters.

## Email selection

1. `SENDGRID_API_KEY` → SendGrid  
2. else `EMAIL_SMTP_HOST` → SMTP (existing)  
3. else DevEmailProvider  

```bash
# SendGrid
set SENDGRID_API_KEY=SG....
set SENDGRID_FROM=support@example.com
```

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
