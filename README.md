# Customer Support CRM

Spec-driven microservices + Angular micro-frontends.

- Backend: .NET 9 (vertical slice / CQRS / DDD) and NestJS (channels, notifications)
- Frontend: Angular 19 Native Federation
- Edge: `http://localhost:5000` only
- Stories: `CRM-001`…`CRM-044` in [specs/STORIES.md](specs/STORIES.md)
- Workflow: [specs/WORKFLOW.md](specs/WORKFLOW.md)

## Demo

```powershell
pwsh -File scripts/dev.ps1
```

Open [http://localhost:5000](http://localhost:5000).

- User: `agent@crm.local`
- Password: `Crm!123`

## First slice

`specs/001-platform-foundation` — gateway, bilingual shell, agent remote, health, Customers `GetBootstrapStatus` query.

Next: `002-customer-profiles` (CRM-001…003).
