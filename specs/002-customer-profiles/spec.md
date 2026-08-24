# Feature Specification: Customer profiles

**Story**: CRM-001, CRM-002, CRM-003  
**Epic**: Customer Management  
**Priority**: Must  
**Status**: Implemented  
**Created**: 2026-08-24

## User story

**As a** support agent,  
**I want** to create, search, and update customer profiles with contacts, notes, and attachments,  
**so that** every conversation starts from a single accurate customer record.

## Business value

Agents stop hunting across channels for who the customer is and how to reach them.

## Scope

**In scope**
- CRM-001: create, view, edit, search profiles (name, organization, status, unique identifier); duplicate unique-id warning
- CRM-002: multiple contacts (email, phone, WhatsApp, address); primary/secondary; deactivate
- CRM-003: chronological timeline of notes and attachments on the customer; add notes; upload/download attachments
- agent-mfe screens under `/agent/customers`
- Seeded demo customers for UAT

**Out of scope**
- Marketing / billing accounts
- Third-party contact enrichment
- Call recording / speech-to-text
- Ticket/message timeline entries (arrive when Tickets/Channels ship); timeline shows notes + attachments for now
- Real RBAC beyond “signed-in agent” (CRM-035)

## Preconditions

- Platform foundation running at `http://localhost:5000`
- Agent signed in: `agent@crm.local` / `Crm!123`

## Screens

| Screen | URL | Actor | Observable result |
|---|---|---|---|
| Customer list | `http://localhost:5000/agent/customers` | agent | Searchable list; Create customer |
| Create customer | `http://localhost:5000/agent/customers/new` | agent | Form; duplicate unique id blocks save with warning |
| Customer detail | `http://localhost:5000/agent/customers/{id}` | agent | Profile, contacts, notes, attachments, timeline |
| Edit customer | `http://localhost:5000/agent/customers/{id}/edit` | agent | Update name, organization, status, unique id |

## Acceptance criteria

### CRM-001 Maintain customer profiles

1. **Given** I am signed in as an agent, **when** I create a customer with a name and unique identifier, **then** the profile is saved and appears in search.
2. **Given** an existing customer, **when** I update profile fields, **then** the latest values are shown on the profile.
3. **Given** a unique identifier that already exists, **when** I try to create another customer with it, **then** the system warns of a possible duplicate and does not save.

### CRM-002 Manage customer contact details

4. **Given** a customer profile, **when** I add email, phone, WhatsApp, or address contacts, **then** they are stored and can be marked primary or secondary.
5. **Given** a contact is no longer valid, **when** I deactivate it, **then** it is marked inactive and not shown as an active contact for outbound use.
6. **Given** I open the customer detail, **when** the page loads, **then** current (active) contact details are visible.

### CRM-003 Notes and attachments (timeline)

7. **Given** a customer has notes or attachments, **when** I open the profile, **then** I see a chronological timeline of those items.
8. **Given** I add a note, **when** I save, **then** it appears on the customer record for agents.
9. **Given** I upload an attachment, **when** it is saved, **then** it is linked to the customer and downloadable.

## Definition of Done

- [x] Acceptance criteria pass in UAT on the gateway
- [x] Automated tests `[Trait("Story", "CRM-001")]` (and CRM-002 / CRM-003 where applicable)
- [x] Seeded demo customers make screens clickable without manual setup
- [x] Spec, plan, and code cite `CRM-001` / `CRM-002` / `CRM-003`

## Assumptions and dependencies

- Depends on: `001-platform-foundation`
- Assumptions: SQLite local store; attachments stored as files under the Customers service data folder; ticket/channel timeline events deferred
