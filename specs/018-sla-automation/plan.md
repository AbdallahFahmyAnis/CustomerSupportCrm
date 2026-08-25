# Implementation Plan: SLA auto-assign and escalation

**Spec**: `specs/018-sla-automation/spec.md`  
**Story**: CRM-018, CRM-019  
**Workflow stage after this plan**: Apply → Test → Mock → Done

## Summary

Extend SLA with auto-assign rules and escalation settings; Tickets applies suggestions on create and via `POST /api/tickets/{id}/run-automation`. Admin SLA page gains rule/settings editors; agent detail gets Run automation.

## Technical Context

**Language/Version**: .NET 9 / Angular  
**Edge**: `http://localhost:5000`  
**Owning service**: SLA (rules) + Tickets (apply)  
**Owning MFE**: admin-mfe (rules); agent-mfe (run automation)

## Constitution Check

- [x] Spec exists with screens + AC
- [x] One vertical slice (automation; SLA owns rules, Tickets mutates tickets)
- [x] No new public port
- [x] Story id in new types/endpoints
- [x] Command/query in feature folders

## Code to apply *(mandatory)*

| Area | Path | Story |
|---|---|---|
| Contracts | `contracts/csharp/Crm.Contracts/Sla/*.cs` | CRM-018/019 |
| SLA API | `src/services/sla/Crm.Sla.Api/Features/{Assign,Escalate}/**` + persistence | CRM-018/019 |
| Tickets | CreateTicket HttpClient suggest; `RunAutomation` feature | CRM-018/019 |
| Admin UI | `admin-mfe/.../features/sla/**` | CRM-018/019 |
| Agent UI | ticket-detail Run automation | CRM-019 |
| Tests | `tests/Crm.Sla.Api.Tests`, `tests/Crm.Tickets.Api.Tests` | traits |
| Product | `specs/000-product/spec.md` | |

## Endpoints

| Service | Method | Path |
|---|---|---|
| SLA | GET/PUT | `/api/sla/assign-rules` |
| SLA | POST | `/api/sla/suggest-assignee` |
| SLA | GET/PUT | `/api/sla/escalation-settings` |
| SLA | POST | `/api/sla/should-escalate` |
| Tickets | POST | `/api/tickets/{id}/run-automation` |

## Seed

- Assign: Technical→Demo; Urgent→Lead; High→Lead; catch-all→Demo (specificity: category+priority > one dimension > default)
- Escalation: Lead `2222…`, escalate on both breaches + Urgent always

## Fail-open

Tickets create/run-automation continue if SLA HTTP fails.
