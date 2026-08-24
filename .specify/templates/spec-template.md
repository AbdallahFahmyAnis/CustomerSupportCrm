# Feature Specification: [FEATURE NAME]

**Story**: CRM-nnn  
**Epic**: [Epic name]  
**Priority**: Must / Should / Could  
**Status**: Draft  
**Created**: [DATE]

## User story

**As a** [role],  
**I want** [capability],  
**so that** [measurable business outcome].

## Business value

[One sentence: risk reduced, revenue protected, or time saved.]

## Scope

**In scope**
- [capability]

**Out of scope**
- [explicit exclusions]

## Preconditions

- Caller is authenticated via the gateway BFF cookie unless this story *is* sign-in.
- Seeded demo: `agent@crm.local` / `Crm!123` at `http://localhost:5000`.

## Screens

| Screen | URL | Actor | Observable result |
|---|---|---|---|
| [Name] | `http://localhost:5000/...` | [role] | [what a BA would tick in UAT] |

## Acceptance criteria

1. **Given** [precondition], **when** [action], **then** [outcome].
2. **Given** [error/empty state], **when** [action], **then** [safe behaviour].

## Definition of Done

- [ ] Acceptance criteria pass in UAT on the gateway
- [ ] Automated test `[Trait("Story", "CRM-nnn")]` where applicable
- [ ] Mock/seed data makes the screens clickable
- [ ] Spec, plan, and code cite `CRM-nnn`

## Assumptions and dependencies

- Depends on: [CRM-nnn]
- Assumptions: [list]
