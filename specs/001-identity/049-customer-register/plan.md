# Implementation Plan: Customer self-registration

**Spec**: `specs/001-identity/049-customer-register/spec.md`  
**Story**: CRM-045  
**Workflow stage after this plan**: Apply → Test → Mock → Done

## Summary

Add anonymous customer registration: Identity creates a Customer-only user and returns tokens; Gateway `/register` sets BFF cookies like login and best-effort creates a Customers profile (`UniqueIdentifier` = email); shell `/register` form with EN/AR links from login.

## Technical Context

**Language/Version**: .NET 9 / Angular  
**Edge**: `http://localhost:5000`  
**Owning service**: Identity (+ Gateway BFF, Customers create)  
**Owning MFE**: shell  

## Constitution Check

- [x] Spec exists with screens + AC
- [x] One vertical slice (register)
- [x] No new public port
- [x] Story id in new types/endpoints
- [x] Command/query live in a feature folder (VSA + CQRS)

## Code to apply *(mandatory)*

| Area | Path | Story |
|---|---|---|
| Contracts | `contracts/.../Identity/IdentityDtos.cs` — `RegisterCustomerRequest` | CRM-045 |
| API | `Identity/.../Features/Auth/RegisterCustomer/*` | CRM-045 |
| Gateway | `Crm.Gateway/Program.cs` — `POST /register` + Customers create | CRM-045 |
| UI | shell register page + routes + SessionApi.register + login links + i18n | CRM-045 |
| Tests | `Crm.Identity.Api.Tests` — `[Trait("Story","CRM-045")]` | CRM-045 |
