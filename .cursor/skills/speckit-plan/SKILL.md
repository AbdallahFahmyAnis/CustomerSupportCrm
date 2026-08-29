---
name: speckit-plan
description: Write the technical plan for the active CRM spec (specs/NNN-hub/MMM-slug/plan.md). Use when the user says plan, after specify, or before implement.
---

# Plan

## Steps

1. Read the active feature from `.specify/feature.json` and its `spec.md`.
2. Read the constitution. Fail the Constitution Check if the slice spans unrelated services or opens a new public port.
3. Copy `.specify/templates/plan-template.md` to `plan.md` in the feature folder.
4. Name owning service, MFE, tables/endpoints, CQRS command vs query, and seed risks (SQLite FK, Guid TEXT, DateTimeOffset ORDER BY).
5. List concrete file paths that will change.

Do not write application code here.
