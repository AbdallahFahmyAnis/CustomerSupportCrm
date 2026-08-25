# SLA service

**Story**: CRM-017 (`specs/017-sla-targets`)

.NET API on `http://localhost:5105` (gateway `/api/sla/...`).

## Endpoints

| Method | Path | Purpose |
|---|---|---|
| GET | `/health` | Health |
| GET | `/api/sla/policies` | List priority targets |
| PUT | `/api/sla/policies/{priority}` | Update first-response / resolution minutes |
| POST | `/api/sla/evaluate` | Due times + breach flags for a ticket snapshot |

## Local run

```powershell
dotnet run --project src/services/sla/Crm.Sla.Api --urls http://localhost:5105
```

Default persistence: Sqlite under `data/sla-ef.db`. Optional SQL Server via `ConnectionStrings:Sla` / `Sla:Provider=SqlServer`.
