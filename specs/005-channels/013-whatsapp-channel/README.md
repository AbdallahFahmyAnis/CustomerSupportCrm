# WhatsApp channel (013 / CRM-009 / CRM-040 stub)

## Ingest (dev provider)

Through the gateway:

```bash
curl -s -X POST http://localhost:5000/api/channels/intake/whatsapp \
  -H "Content-Type: application/json" \
  -d "{\"from\":\"+15551234567\",\"body\":\"My order is late.\",\"name\":\"Ada\"}"
```

Returns `{ requestId, ticketId, ticketNumber }`. Open the ticket in agent-mfe to see **Channel messages** (`WhatsApp` / `Inbound`).

## Reply

```bash
curl -s -X POST http://localhost:5000/api/channels/tickets/<ticketId>/messages/whatsapp \
  -H "Content-Type: application/json" \
  -d "{\"body\":\"Sorry about the delay — tracking soon.\"}"
```

Or use **WhatsApp reply** on the agent ticket detail screen.

## Provider

`DevWhatsAppProvider` implements `WhatsAppProvider` — swap later for Meta/Twilio without changing the CQRS command.
