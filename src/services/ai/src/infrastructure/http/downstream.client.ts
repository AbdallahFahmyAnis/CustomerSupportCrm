import { Injectable } from '@nestjs/common';
import { aiConfig } from '../../app/config';

/** SDD CRM-023 — ticket shape used by AI heuristics. */
export type TicketSnapshot = {
  id: string;
  ticketNumber: string;
  subject: string;
  description?: string | null;
  category: string;
  priority: string;
  status: string;
  customerName: string;
};

/** SDD CRM-023 — HTTP to Tickets (+ Knowledge for later slices). */
@Injectable()
export class DownstreamClient {
  private readonly ticketsBase = aiConfig.ticketsUrl;
  private readonly knowledgeBase = aiConfig.knowledgeUrl;

  async getTicket(id: string): Promise<TicketSnapshot | null> {
    const res = await fetch(`${this.ticketsBase}/api/tickets/${id}`);
    if (res.status === 404) return null;
    if (!res.ok) throw new Error(`Tickets GET failed: ${res.status}`);
    return (await res.json()) as TicketSnapshot;
  }

  /** SDD CRM-023 polish / 042 — persist summary on ticket row. */
  async saveAiSummary(
    ticketId: string,
    summary: string,
    highlights: string[],
  ): Promise<void> {
    const res = await fetch(`${this.ticketsBase}/api/tickets/${ticketId}/ai-summary`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ summary, highlights }),
    });
    if (!res.ok) throw new Error(`Tickets PUT ai-summary failed: ${res.status}`);
  }

  async searchKnowledge(q: string): Promise<{ id: string; title: string; snippet?: string }[]> {
    const qs = new URLSearchParams({ q: q.trim() || 'help' });
    const res = await fetch(`${this.knowledgeBase}/api/knowledge/search?${qs}`);
    if (!res.ok) return [];
    const body = (await res.json()) as { id: string; title: string; snippet?: string }[];
    return Array.isArray(body) ? body.slice(0, 5) : [];
  }

  async listPortalFaqs(q = ''): Promise<{ id: string; title: string }[]> {
    // Prefer unfiltered catalog so heuristics can rank; fall back to token search if needed.
    const res = await fetch(`${this.knowledgeBase}/api/knowledge/portal/faqs`);
    if (!res.ok) return [];
    const body = (await res.json()) as { id: string; title: string }[];
    const all = Array.isArray(body) ? body : [];
    if (!q.trim() || all.length === 0) return all.slice(0, 12);
    return all.slice(0, 12);
  }
}
