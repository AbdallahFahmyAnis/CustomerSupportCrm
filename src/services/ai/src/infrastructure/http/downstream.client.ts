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

  async searchKnowledge(q: string): Promise<{ id: string; title: string; snippet?: string }[]> {
    const qs = new URLSearchParams({ q: q.trim() || 'help' });
    const res = await fetch(`${this.knowledgeBase}/api/knowledge/search?${qs}`);
    if (!res.ok) return [];
    const body = (await res.json()) as { id: string; title: string; snippet?: string }[];
    return Array.isArray(body) ? body.slice(0, 5) : [];
  }

  async listPortalFaqs(q = ''): Promise<{ id: string; title: string }[]> {
    const qs = q.trim() ? `?q=${encodeURIComponent(q.trim())}` : '';
    const res = await fetch(`${this.knowledgeBase}/api/knowledge/portal/faqs${qs}`);
    if (!res.ok) return [];
    const body = (await res.json()) as { id: string; title: string }[];
    return Array.isArray(body) ? body.slice(0, 5) : [];
  }
}
