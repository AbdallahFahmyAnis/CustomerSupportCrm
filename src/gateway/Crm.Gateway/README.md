# CRM Gateway

BFF edge at `http://localhost:5000` (cookie session + YARP).

## External API (CRM-038)

Machine clients call `/api/external/v1/*` with a static API key (not cookie auth):

| Header | Example |
|---|---|
| `X-Api-Key` | `dev-external-key` |
| or `Authorization` | `ApiKey dev-external-key` |

Configured as `ExternalApi:ApiKey` in `appsettings.json` (demo value only).

| Method | Path | Downstream |
|---|---|---|
| `POST` | `/api/external/v1/tickets` | Tickets create |
| `GET` | `/api/external/v1/tickets/{id}` | Tickets get |
| `GET` | `/api/external/v1/customers?q=` | Customers search |
