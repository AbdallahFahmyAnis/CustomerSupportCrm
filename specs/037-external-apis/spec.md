# Feature Specification: External APIs

**Story**: CRM-038  
**Epic**: Integrations  
**Priority**: Should  
**Status**: Implemented  
**Created**: 2026-08-25

## User story

**As an** external system integrator,  
**I want** a small authenticated HTTP API for tickets and customers,  
**so that** I can create/lookup work without a browser session.

## Scope

- Gateway `/api/external/v1/*` with static API key (`X-Api-Key` or `Authorization: ApiKey …`)
- Proxy to existing Tickets create/get and Customers search
- Demo key `dev-external-key` in appsettings

## Out of scope

- OpenAPI UI, OAuth client-credentials, rate limits

## Definition of Done

- [x] AC + test citing CRM-038
- [x] Spec/plan/code cite CRM-038
