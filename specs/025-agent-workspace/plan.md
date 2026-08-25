# Implementation Plan: Agent workspace with customer context

**Spec**: `specs/025-agent-workspace/spec.md`  
**Story**: CRM-013  

## Summary

Enhance agent-mfe home and ticket list/detail to surface assigned queue and an inline customer summary using existing Tickets search and Customers GET APIs.

## Technical Context

**Owning MFE**: agent-mfe  
**APIs**: `GET /api/tickets?assignedTo=`, `GET /api/customers/{id}`

## Code to apply

| Area | Path |
|---|---|
| Spec | `specs/025-agent-workspace/*` |
| Home | `features/home/home/*` |
| List | `ticket-list.page.ts` default mine |
| Detail | customer summary on `ticket-detail/*` |
| Product | `specs/000-product/spec.md` |
