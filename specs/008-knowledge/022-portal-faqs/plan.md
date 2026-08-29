# Implementation Plan: Portal FAQ access

**Spec**: `specs/008-knowledge/022-portal-faqs/spec.md`  
**Story**: CRM-029  
**Workflow stage after this plan**: Apply â†’ Test â†’ Mock â†’ Done

## Summary

Expose portal-safe FAQ list/detail on Knowledge (Published Faq only), and add portal-mfe browse + detail screens reachable from the portal home.

## Technical Context

**Language/Version**: .NET 9 / Angular  
**Edge**: `http://localhost:5000`  
**Owning service**: Knowledge  
**Owning MFE**: portal-mfe

## Constitution Check

- [x] Spec exists with screens + AC
- [x] One vertical slice (portal FAQ surface)
- [x] No new public port
- [x] Story id in new types/endpoints
- [x] VSA + CQRS

## Code to apply *(mandatory)*

| Area | Path | Story |
|---|---|---|
| API | `Features/PortalFaqs/ListPortalFaqs/**` | CRM-029 |
| API | `Features/PortalFaqs/GetPortalFaq/**` | CRM-029 |
| Domain/Infra | `KnowledgeDb` portal FAQ helpers | CRM-029 |
| portal-mfe | `features/faqs/**` | CRM-029 |
| portal-mfe | portal home tile + routes | CRM-029 |
| Tests | `Crm.Knowledge.Api.Tests` | CRM-029 |
| Docs | Knowledge README + product Next | CRM-029 |

## Endpoints

| Method | Path | Behaviour |
|---|---|---|
| GET | `/api/knowledge/portal/faqs?q=` | Published Faq summaries; optional title/body contains filter |
| GET | `/api/knowledge/portal/faqs/{id}` | Published Faq detail; 404 if missing, Draft, or non-Faq |

Reuse existing summary/detail DTOs from `Crm.Contracts.Knowledge`.

## UI

- Feature `faqs`: `faqs.api.ts`, `faqs.models.ts`, `faqs.store.ts`, `faqs.routes.ts`
- Pages: `faq-list`, `faq-detail` (signals store; gateway `/api/knowledge/portal/...` only)
- Portal home action tile â†’ `/portal/faqs`
