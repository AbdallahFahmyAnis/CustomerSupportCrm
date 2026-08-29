# Customer Support CRM stories

Source: Azure DevOps project [Customer Support CRM](https://dev.azure.com/abdallahfahmy244/Customer%20Support%20CRM/_boards/board/t/Customer%20Support%20CRM%20Team/Stories), imported from `azm_squad_customer_support_crm.pdf`.

Story ids `CRM-nnn` are stable. Azure DevOps work item ids are listed for traceability.

Recommended SDD order after platform foundation: **002 Customers (CRM-001…003)** → **003 Tickets (CRM-004…007)** → **004 Identity (CRM-035)** → channels / portal.

## Platform

| Id | WI | Title | Priority | Owner |
|---|---|---|---|---|
| CRM-041 | 55 | Use the CRM in Arabic and English | Must | shell |
| CRM-042 | 56 | Use a web and mobile-friendly interface | Must | shell |
| CRM-043 | 57 | Support multiple departments and branches | Should | Identity |
| CRM-044 | 58 | Apply custom branding | Could | shell / Identity |

Epic WI: 54.

## Customer Management

| Id | WI | Title | Priority | Owner |
|---|---|---|---|---|
| CRM-001 | 4 | Maintain customer profiles | Must | Customers + agent-mfe |
| CRM-002 | 5 | Manage customer contact details | Must | Customers + agent-mfe |
| CRM-003 | 6 | Record interaction history, notes, and attachments | Must | Customers + agent-mfe |

Epic WI: 3.

## Ticket Management

| Id | WI | Title | Priority | Owner |
|---|---|---|---|---|
| CRM-004 | 8 | Create and track support tickets | Must | Tickets + agent-mfe |
| CRM-005 | 9 | Categorize and prioritize tickets | Must | Tickets + agent-mfe |
| CRM-006 | 10 | Assign tickets to agents | Must | Tickets + agent-mfe |
| CRM-007 | 11 | Manage ticket status, escalation, and history | Must | Tickets + agent-mfe |

Epic WI: 7.

## Communication Channels

| Id | WI | Title | Priority | Owner |
|---|---|---|---|---|
| CRM-008 | 13 | Handle customer email as tickets | Must | Channels + Tickets |
| CRM-009 | 14 | Handle WhatsApp conversations as tickets | Should | Channels + Tickets |
| CRM-010 | 15 | Handle live chat as tickets | Should | Channels + Tickets |
| CRM-011 | 16 | Send and receive SMS on tickets | Should | Channels + Tickets |
| CRM-012 | 17 | Capture support requests from web forms | Must | Channels + Tickets |

Epic WI: 12.

## Agent Dashboard

| Id | WI | Title | Priority | Owner |
|---|---|---|---|---|
| CRM-013 | 19 | Work assigned tickets with customer context | Must | agent-mfe (Tickets + Customers) |
| CRM-014 | 20 | Manage tasks and reminders | Should | Tickets + agent-mfe |
| CRM-015 | 21 | Use quick replies while handling tickets | Should | Tickets + agent-mfe |
| CRM-016 | 22 | Collaborate with the team on a ticket | Should | Tickets + Notifications |

Epic WI: 18.

## SLA and Automation

| Id | WI | Title | Priority | Owner |
|---|---|---|---|---|
| CRM-017 | 24 | Define response and resolution SLA targets | Should | SLA |
| CRM-018 | 25 | Automatically assign tickets | Should | SLA + Tickets |
| CRM-019 | 26 | Apply escalation rules | Should | SLA + Tickets |
| CRM-020 | 27 | Receive alerts and notifications | Should | Notifications |

Epic WI: 23.

## Knowledge Base

| Id | WI | Title | Priority | Owner |
|---|---|---|---|---|
| CRM-021 | 29 | Author FAQs, articles, solutions, and guides | Should | Knowledge + knowledge-mfe |
| CRM-022 | 30 | Search the knowledge base | Should | Knowledge + knowledge-mfe |

Epic WI: 28.

## AI Features

| Id | WI | Title | Priority | Owner |
|---|---|---|---|---|
| CRM-023 | 32 | Generate ticket summaries | Could | AI |
| CRM-024 | 33 | Get suggested replies and solutions | Could | AI |
| CRM-025 | 34 | Automatically categorize tickets | Could | AI |
| CRM-026 | 35 | Provide an AI chatbot for common questions | Could | AI + portal-mfe |

Epic WI: 31.

## Customer Portal

| Id | WI | Title | Priority | Owner |
|---|---|---|---|---|
| CRM-027 | 37 | Submit tickets from the customer portal | Must | Tickets + portal-mfe |
| CRM-028 | 38 | Track requests and view history | Must | Tickets + portal-mfe |
| CRM-029 | 39 | Access FAQs from the customer portal | Should | Knowledge + portal-mfe |
| CRM-030 | 40 | Submit customer feedback | Should | Tickets + portal-mfe |
| CRM-045 | — | Register a customer portal account | Must | Identity + shell + portal-mfe (+ Customers link) |

Epic WI: 36.

## Reports and Management

| Id | WI | Title | Priority | Owner |
|---|---|---|---|---|
| CRM-031 | 42 | View ticket reports | Should | Tickets (read models) + admin-mfe |
| CRM-032 | 43 | Monitor SLA and agent performance | Should | SLA + admin-mfe |
| CRM-033 | 44 | Track customer satisfaction | Should | Tickets + admin-mfe |
| CRM-034 | 45 | Use management dashboards | Should | admin-mfe |

Epic WI: 41.

## Security and Administration

| Id | WI | Title | Priority | Owner |
|---|---|---|---|---|
| CRM-035 | 47 | Manage users, roles, and permissions | Must | Identity + admin-mfe |
| CRM-036 | 48 | Review audit logs | Must | Identity + admin-mfe |
| CRM-037 | 49 | Configure the system | Must | Identity + admin-mfe |
| CRM-046 | — | Reset a forgotten password | Must | Identity + shell (+ email outbound) |

Feature map: [`specs/identity/feature.md`](identity/feature.md) (CRM-035…037, 043–046).  
All hubs: [`specs/README.md`](README.md).

Epic WI: 46.

## Integrations

| Id | WI | Title | Priority | Owner |
|---|---|---|---|---|
| CRM-038 | 51 | Expose APIs for external systems | Should | Gateway + owning services |
| CRM-039 | 52 | Integrate with ERP and other external systems | Could | Integrations (later) |
| CRM-040 | 53 | Integrate email, SMS, and WhatsApp providers | Should | Channels |

Epic WI: 50.
