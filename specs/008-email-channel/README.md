# Email channel (008 / CRM-008 / CRM-040 stub)

## Ingest (dev provider)

Through the gateway:

```bash
curl -s -X POST http://localhost:5000/api/channels/intake/email \
  -H "Content-Type: application/json" \
  -d "{\"from\":\"customer@example.com\",\"subject\":\"Need help\",\"body\":\"My order is late.\"}"
```

Returns `{ requestId, ticketId, ticketNumber }`. Open the ticket in agent-mfe to see **Channel messages** (`Email` / `Inbound`).

## Provider

`DevEmailProvider` implements `EmailProvider` — swap later for SendGrid/IMAP without changing the CQRS command.
