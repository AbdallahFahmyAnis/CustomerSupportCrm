# Feature Specification: SLA and agent performance

**Story**: CRM-032  
**Epic**: Reports and Management  
**Priority**: Should  
**Status**: Implemented  
**Created**: 2026-08-25

## User story

**As a** manager,  
**I want** SLA breach rates and agent performance,  
**so that** I can coach the team and protect response commitments.

## Scope

**In scope**
- `GET /api/tickets/reports/sla-performance?from=&to=`
- admin-mfe `/admin/reports/sla`
- Resolution breach using demo SLA policy minutes (mirrors SLA seed)

**Out of scope**
- Live HTTP to SLA service for each ticket; first-response timestamps column

## Definition of Done

- [x] AC + automated test CRM-032
- [x] Spec/plan/code cite CRM-032
