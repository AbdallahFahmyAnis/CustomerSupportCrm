# Feature Specification: Ticket reports

**Story**: CRM-031  
**Epic**: Reports and Management  
**Priority**: Should  
**Status**: Implemented  
**Created**: 2026-08-25

## User story

**As a** manager,  
**I want** ticket volume reports by status, category, priority, and agent,  
**so that** I can see workload and trends.

## Scope

**In scope**
- `GET /api/tickets/reports/summary?from=&to=`
- admin-mfe `/admin/reports` with date filter and count tables

**Out of scope**
- Export/CSV; realtime charts libraries

## Acceptance criteria

1. Given tickets in a date range, when I request the summary, then totals and bucket counts match.
2. Given I open `/admin/reports`, when I set dates, then the summary tables load.

## Definition of Done

- [x] AC + automated test CRM-031
- [x] Spec/plan/code cite CRM-031
