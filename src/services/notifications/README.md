# Notifications service

**Story**: CRM-020 (`specs/021-notifications`)

NestJS on `http://localhost:5202` (gateway `/api/notifications/...`).

## Endpoints

| Method | Path | Purpose |
|---|---|---|
| GET | `/health` | Health |
| GET | `/api/notifications` | Inbox for `X-Crm-User-Id` |
| GET | `/api/notifications/unread-count` | Badge count |
| POST | `/api/notifications/:id/read` | Mark read |

## Local run

```powershell
npm start
```

JSON store: `data/notifications-store.json` (or `NOTIFICATIONS_DATA_PATH`).
