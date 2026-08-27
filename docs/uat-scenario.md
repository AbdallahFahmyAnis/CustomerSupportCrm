# UAT scenario — Customer Support CRM (all services)

**Edge:** http://localhost:5000  
**Start:** `pwsh -File scripts/dev.ps1`  
**Wait:** ~1–2 minutes until gateway + MFEs are up; check http://localhost:5000/health  

| Role | Email | Password |
|---|---|---|
| Agent | `agent@crm.local` | `Crm!123` |
| Admin | `admin@crm.local` | `Crm!123` |
| External API key | Header `X-Api-Key: dev-external-key` | — |

Mark each step **Pass / Fail / Blocked** as you go.

---

## 0. Smoke — platform is up

| # | Action | Expected | Service(s) |
|---|---|---|---|
| 0.1 | Open http://localhost:5000 | Login / shell loads | Gateway, shell |
| 0.2 | Open http://localhost:5000/health | JSON with services healthy (or degraded only if optional down) | Gateway → all backends |
| 0.3 | Toggle language EN ↔ AR | Chrome flips LTR/RTL | shell (CRM-041) |

---

## 1. Admin — identity, config, branding (admin@crm.local)

Sign in as **admin@crm.local**.

| # | Screen / path | Action | Expected | Service(s) |
|---|---|---|---|---|
| 1.1 | `/admin/users` | List users; create or deactivate a test user if UI allows | Users list works | Identity, admin-mfe |
| 1.2 | `/admin/roles` | Open roles / permissions | Roles visible | Identity |
| 1.3 | `/admin` settings | Set org name, lockout, product title, primary color, logo URL | Saves; shell branding reflects after refresh | Identity (CRM-037/044) |
| 1.4 | Settings | Note **ERP webhook URL** (leave empty for now) and auth header | Fields save | Identity + Tickets later |
| 1.5 | Audit logs | Open audit list | Recent settings/user actions appear | Identity (CRM-036) |
| 1.6 | Departments | Create a department (and branch if UI shows) | Appears in list | Identity (CRM-043) |
| 1.7 | Dashboard / reports | Open management KPI / ticket / SLA / CSAT reports | Charts or tables load (may be sparse) | Tickets, SLA, admin-mfe (CRM-031…034) |

Sign out.

---

## 2. Agent — customers & tickets (agent@crm.local)

Sign in as **agent@crm.local**.

### 2.1 Customers (CRM-001…003)

| # | Screen | Action | Expected | Service(s) |
|---|---|---|---|---|
| 2.1.1 | `/agent/customers` | Search / open seeded **Acme** (or create “UAT Co”) | Profile loads | Customers |
| 2.1.2 | Customer detail | Add contact / note / attachment if UI allows | Persists after refresh | Customers |

### 2.2 Ticket lifecycle (CRM-004…007, 013)

| # | Screen | Action | Expected | Service(s) |
|---|---|---|---|---|
| 2.2.1 | Create ticket | Subject “UAT invoice mismatch”, category Billing, High, link customer | Ticket number `TKT-…` created | Tickets |
| 2.2.2 | Ticket detail | Change classification / assign to Demo Agent / change status | History updates | Tickets |
| 2.2.3 | Escalate | Escalate if control present | Escalated flag / assignee change | Tickets |
| 2.2.4 | My tickets / home | Filter assigned to me | Ticket appears | Tickets, agent-mfe |

### 2.3 Collaboration & productivity (CRM-014…016, 015)

| # | Screen | Action | Expected | Service(s) |
|---|---|---|---|---|
| 2.3.1 | Notes | Add note with `@` mention if suggested | Note saved | Tickets (+ Notifications) |
| 2.3.2 | Tasks | Add follow-up task with due date | Task on ticket / due-today | Tickets |
| 2.3.3 | Quick replies | Insert a canned reply into email/chat draft | Body filled | Tickets |

### 2.4 Knowledge on ticket (CRM-022)

| # | Screen | Action | Expected | Service(s) |
|---|---|---|---|---|
| 2.4.1 | Knowledge panel | Search “password” or “billing” | Ranked hits | Knowledge |

### 2.5 AI on ticket (CRM-023…025)

| # | Screen | Action | Expected | Service(s) |
|---|---|---|---|---|
| 2.5.1 | Generate summary | Click generate; watch text stream in | Summary + highlights; survives reload | AI, Tickets |
| 2.5.2 | Suggest replies | Click suggest | 2–3 reply stubs | AI |
| 2.5.3 | Auto-categorize | Click categorize | Category/priority suggestion | AI |

### 2.6 Channel reply (CRM-008…011)

Use an existing ticket or intake below first.

| # | Screen | Action | Expected | Service(s) |
|---|---|---|---|---|
| 2.6.1 | Email / chat panel | Send outbound reply | Message appears in thread | Channels |

### 2.7 Notifications (CRM-020)

| # | Screen | Action | Expected | Service(s) |
|---|---|---|---|---|
| 2.7.1 | Shell bell | Open inbox after note/mention or assign | Notification listed | Notifications |

### 2.8 SLA (CRM-017…019)

| # | Screen | Action | Expected | Service(s) |
|---|---|---|---|---|
| 2.8.1 | Ticket detail | View SLA due / breach indicators if shown | Evaluation returns | SLA |
| 2.8.2 | Run automation | Click if present | Possible auto-assign/escalate | SLA, Tickets |

---

## 3. Channels intake (API via gateway — curl or Postman)

Run against **http://localhost:5000** (cookie optional for these intake stubs).

### 3.1 Web form / email / WhatsApp / chat / SMS

Replace placeholders after first create.

```powershell
# Email intake
curl -s -X POST http://localhost:5000/api/channels/intake/email `
  -H "Content-Type: application/json" `
  -d "{\"from\":\"uat.customer@example.com\",\"subject\":\"UAT email case\",\"body\":\"Need help with invoice\"}"

# WhatsApp
curl -s -X POST http://localhost:5000/api/channels/intake/whatsapp `
  -H "Content-Type: application/json" `
  -d "{\"from\":\"+15550001111\",\"body\":\"UAT WhatsApp hello\"}"

# Live chat
curl -s -X POST http://localhost:5000/api/channels/intake/chat `
  -H "Content-Type: application/json" `
  -d "{\"sessionId\":\"uat-session-1\",\"body\":\"UAT live chat hello\"}"

# SMS
curl -s -X POST http://localhost:5000/api/channels/intake/sms `
  -H "Content-Type: application/json" `
  -d "{\"from\":\"+15550002222\",\"body\":\"UAT SMS hello\"}"
```

| # | Check | Expected | Service(s) |
|---|---|---|---|
| 3.1 | Each intake returns ticket / message ids | 2xx + ticketId | Channels, Customers, Tickets |
| 3.2 | Open ticket in agent UI | Channel messages visible | Channels, agent-mfe |

---

## 4. Portal — customer journeys (no login or portal user as designed)

Open http://localhost:5000 → **Portal** (or `/portal`).

| # | Screen | Action | Expected | Service(s) |
|---|---|---|---|---|
| 4.1 | `/portal/submit` | Submit a request | Confirmation / trackable id | Channels/Tickets, portal-mfe |
| 4.2 | `/portal/track` | Track with seeded email if shown (`portal.customer@example.com`) | History list | Channels |
| 4.3 | `/portal/faqs` | Browse FAQ; open one | Detail page | Knowledge (CRM-029) |
| 4.4 | Feedback / CSAT | Submit rating on a closed/eligible ticket if UI allows | Saved; agent can see read-only | Tickets (CRM-030) |
| 4.5 | `/portal/assistant` | Ask “password reset”; then “tell me more”; then “I want a human agent” | FAQ reply → follow-up memory → handoff CTA | AI (CRM-026) |

---

## 5. Knowledge authoring (knowledge-mfe)

| # | Screen | Action | Expected | Service(s) |
|---|---|---|---|---|
| 5.1 | `/knowledge` (or shell nav) | Create/publish a FAQ or article | Appears in search / portal FAQs | Knowledge, knowledge-mfe |

Sign in as agent/admin if the MFE requires session.

---

## 6. Integrations

### 6.1 External API (CRM-038)

```powershell
curl -s http://localhost:5000/api/external/v1/docs
# Browser: http://localhost:5000/api/external/v1/docs

curl -s http://localhost:5000/api/external/v1/openapi.yaml | Select-Object -First 5

curl -s http://localhost:5000/api/external/v1/customers?q=Acme `
  -H "X-Api-Key: dev-external-key"
```

| # | Check | Expected |
|---|---|---|
| 6.1.1 | Swagger UI loads | Try-it-out works with key |
| 6.1.2 | GET customers with key | 200 + rows |
| 6.1.3 | Without key | 401 |

### 6.2 ERP webhook (CRM-039)

1. As **admin**, set ERP webhook URL to a catcher (e.g. https://webhook.site/… or a local listener).  
2. Optional: Authorization header `Bearer uat-token`.  
3. Create a new ticket as agent.  
4. Refresh **ERP outbox** on settings.

| # | Check | Expected | Service(s) |
|---|---|---|---|
| 6.2.1 | Empty URL | No delivery / no-op | Tickets |
| 6.2.2 | Valid URL | Catcher receives JSON; outbox shows `ok:…` | Tickets, Identity |
| 6.2.3 | Bad URL / 5xx | Retries then `fail:…` / `error` in outbox | Tickets |

---

## 7. End-of-UAT checklist (by service)

| Service | Port | Covered by steps |
|---|---|---|
| Gateway | 5000 | 0.*, 6.* |
| Identity | 5101 | 1.* |
| Customers | 5102 | 2.1, 3, 6.1 |
| Tickets | 5103 | 2.2–2.3, 2.8, 4.4, 6.2 |
| Knowledge | 5104 | 2.4, 4.3, 5 |
| SLA | 5105 | 2.8, 1.7 |
| Channels | 5201 | 2.6, 3, 4.1–4.2 |
| Notifications | 5202 | 2.7 |
| AI | 5203 | 2.5, 4.5 |
| shell / MFEs | 4200–4204 | All UI steps |

---

## 8. Suggested happy-path script (≈45–60 min)

1. Health + bilingual toggle  
2. Admin settings + branding + department  
3. Agent: open Acme → create ticket → assign → note → task → AI summary stream → knowledge search  
4. Portal: submit → FAQs → assistant (multi-turn + handoff)  
5. One channel intake (email) → open ticket messages  
6. External Swagger Try it out  
7. Optional ERP webhook + outbox  

---

## 9. Known demo limits

- Channel providers are **dev stubs** unless SendGrid/Twilio env is configured.  
- AI is **heuristic** (no external LLM keys).  
- ERP / chat session durability are **local files** under service data paths.  
- Sparse data may make reports look empty — still Pass if pages load without error.

---

*UAT sheet for Customer Support CRM — pair with `docs/hld-report.md`.*
