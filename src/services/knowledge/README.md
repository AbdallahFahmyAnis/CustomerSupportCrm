# Knowledge service

**Story**: CRM-021 (`specs/019-knowledge-authoring`)

.NET API on `http://localhost:5104` (gateway `/api/knowledge/...`).

## Endpoints

| Method | Path | Purpose |
|---|---|---|
| GET | `/health` | Health |
| GET | `/api/knowledge/articles?q=` | List / filter |
| GET | `/api/knowledge/articles/{id}` | Detail |
| POST | `/api/knowledge/articles` | Create |
| PUT | `/api/knowledge/articles/{id}` | Update |

## Local run

```powershell
dotnet run --project src/services/knowledge/Crm.Knowledge.Api --urls http://localhost:5104
```

Default persistence: Sqlite `data/knowledge-ef.db`.
