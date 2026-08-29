# Outbound email reply (009 / CRM-040)

## Agent UI

Open a ticket → **Email reply** → send. Message appears as `Email · Outbound`.

## API

```bash
curl -s -X POST http://localhost:5000/api/channels/tickets/{ticketId}/messages/email \
  -H "Content-Type: application/json" \
  -d "{\"body\":\"Thanks — we are looking into this.\"}"
```

## Providers

| Mode | When | Behavior |
|------|------|----------|
| Dev (default) | no `EMAIL_SMTP_HOST` | Logs send; always succeeds |
| SMTP | `EMAIL_SMTP_HOST` set | Uses nodemailer |

```powershell
$env:EMAIL_SMTP_HOST = "smtp.example.com"
$env:EMAIL_SMTP_PORT = "587"
$env:EMAIL_SMTP_USER = "user"
$env:EMAIL_SMTP_PASS = "pass"
$env:EMAIL_SMTP_FROM = "support@yourcrm.local"
```
