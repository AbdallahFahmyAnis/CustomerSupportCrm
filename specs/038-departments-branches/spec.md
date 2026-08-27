# Feature Specification: Departments and branches

**Story**: CRM-043  
**Epic**: Platform  
**Priority**: Should  
**Status**: Implemented  
**Created**: 2026-08-25

## User story

**As an** administrator,  
**I want** departments and branches with optional user and ticket assignment,  
**so that** work can be organized by org unit.

## Scope

- Identity catalog: Department + Branch (branch → department)
- Admin list/create + user create fields
- Tickets optional `departmentId` on create + list filter

## Out of scope

- Hard RBAC isolation by department

## Definition of Done

- [x] AC + test citing CRM-043
