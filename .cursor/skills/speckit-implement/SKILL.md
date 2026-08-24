---
name: speckit-implement
description: Implement the active CRM spec in dependency order. Use when the user says implement, build it, or proceed to next after a spec exists.
---

# Implement

## Steps

1. Require `specs/NNN-slug/spec.md`. If missing, run specify first.
2. Follow `plan.md` / `tasks.md` if present; otherwise implement P1 stories only.
3. Match existing patterns: nested vertical slices `Features/{Area}/{UseCase}/` with `Endpoint.cs` + `Command|Query.cs` + `Handler.cs` (+ optional Validator/Response); thin `Program.cs` + `DependencyInjection.cs`; MediatR/`@nestjs/cqrs`; Angular Native Federation MFE that calls `/api/...` on the gateway.
4. Check off tasks as you go. Restart the touched service. Verify success criteria at `http://localhost:5000`.
5. Set spec Status to `Implemented`. Update `specs/000-product/spec.md` shipped list if this was a product slice.
6. Commit only when the user asks. Never add db/bin/tmp files.

If the user said proceed to next and no spec exists for the next backlog item, specify that item first in the same session, then implement it.
