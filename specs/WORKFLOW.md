# Qualified story workflow (SDD)

Every Customer Support CRM story moves through the same stages. Specs, plans, code comments, and tests must name the **story id** (`CRM-nnn`) and the spec folder.

## Stages

| Stage | Meaning | Status label | Exit criteria |
|---|---|---|---|
| **Specify** | Finish `spec.md` (persona, screens, AC, mocks) | Spec finished | Reviewer can demo from the spec alone |
| **Apply** | Implement the plan’s file list | In progress | Code comments / types cite `CRM-nnn` |
| **Test** | Automated tests + screen smoke | Testing | Tests with story traits pass; screens checked on the gateway |
| **Mock** | Seed / demo data so the story is clickable | Mock in progress | Seeded agent/customer show the screen without extra setup |
| **Retest** | Repeat tests against mock data | Testing | Smoke on seeded demo at `http://localhost:5000` |
| **Done** | Slice is shipped | Done | Spec status Implemented; Azure DevOps story Done |

Do not skip **Specify**. Do not mark **Done** without **Test** and a **Mock** path.

Azure DevOps holds the backlog (`CRM-nnn`). Specs under `specs/<hub>/NNN-slug` are the executable contract (see `specs/README.md`).

## Code pattern

```csharp
/// <summary>SDD CRM-001 / specs/003-customers/002-customer-profiles.</summary>
```

Angular files use the same ids in a file header comment. Tests use `[Trait("Story", "CRM-001")]`. NestJS handlers use a `/** SDD CRM-nnn */` block comment.
