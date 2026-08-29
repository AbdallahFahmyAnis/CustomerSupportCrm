# SMS channel (015 / CRM-011 / CRM-040 stub)

## Ingest (dev provider)

Through the gateway:

```bash
curl -s -X POST http://localhost:5000/api/channels/intake/sms \
  -H "Content-Type: application/json" \
  -d "{\"from\":\"+15559876543\",\"body\":\"Need a callback about my bill.\",\"name\":\"Sam\"}"
```

Returns `{ requestId, ticketId, ticketNumber }`. Open the ticket in agent-mfe to see **Channel messages** (`Sms` / `Inbound`).

## Reply

```bash
curl -s -X POST http://localhost:5000/api/channels/tickets/<ticketId>/messages/sms \
  -H "Content-Type: application/json" \
  -d "{\"body\":\"Thanks — we will call you shortly.\"}"
```

Or choose **SMS** on the agent ticket detail channel picker and send a reply.

## Provider

`DevSmsProvider` implements `SmsProvider` — swap later for Twilio/SNS without changing the CQRS command.
