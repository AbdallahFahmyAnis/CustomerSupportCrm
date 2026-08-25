/** SDD CRM-023…026 — AI service configuration. */
export const aiConfig = {
  port: Number(process.env.PORT ?? 5203),
  ticketsUrl: process.env.TICKETS_URL ?? 'http://localhost:5103',
  knowledgeUrl: process.env.KNOWLEDGE_URL ?? 'http://localhost:5104',
};
