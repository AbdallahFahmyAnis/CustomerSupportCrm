# Live chat channel (014 / CRM-010)

## Ingest (dev provider)

Through the gateway:

```bash
curl -s -X POST http://localhost:5000/api/channels/intake/chat \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"visitor@example.com\",\"body\":\"Need help with my order.\",\"name\":\"Visitor\"}"
```

Returns `{ requestId, ticketId, ticketNumber }`. Open the ticket in agent-mfe to see **Channel messages** (`LiveChat` / `Inbound`).

## Continue on the same ticket

```bash
curl -s -X POST http://localhost:5000/api/channels/intake/chat \
  -H "Content-Type: application/json" \
  -d "{\"email\":\"visitor@example.com\",\"body\":\"Still waiting.\",\"ticketId\":\"<ticketId>\"}"
```

## Reply

```bash
curl -s -X POST http://localhost:5000/api/channels/tickets/<ticketId>/messages/chat \
  -H "Content-Type: application/json" \
  -d "{\"body\":\"Thanks — looking into it now.\"}"
```

Or use **Live chat reply** on the agent ticket detail screen / portal `/portal/chat`.

## Provider

`DevChatProvider` implements `ChatProvider` — swap later for a realtime vendor without changing the CQRS command.
