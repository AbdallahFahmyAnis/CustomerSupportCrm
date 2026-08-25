import { TicketSnapshot } from '../http/downstream.client';

/** SDD CRM-023 — deterministic summary (no external LLM). */
export function summarizeTicket(ticket: TicketSnapshot): {
  summary: string;
  highlights: string[];
} {
  const desc = (ticket.description || '').trim();
  const first =
    desc.split(/(?<=[.!?])\s+/).filter(Boolean)[0] ||
    ticket.subject ||
    'No details provided.';
  const highlights = [
    `Customer: ${ticket.customerName}`,
    `Status: ${ticket.status}`,
    `Priority: ${ticket.priority}`,
    `Category: ${ticket.category}`,
  ];
  const summary = `${ticket.ticketNumber}: ${ticket.subject}. ${first}`.trim();
  return { summary, highlights };
}

/** SDD CRM-023 deferred / 046 — split summary into streamable token chunks. */
export function streamSummaryChunks(summary: string, chunkSize = 8): string[] {
  const words = summary.trim().split(/\s+/).filter(Boolean);
  if (words.length === 0) return [];
  const chunks: string[] = [];
  for (let i = 0; i < words.length; i += chunkSize) {
    const slice = words.slice(i, i + chunkSize).join(' ');
    chunks.push(i === 0 ? slice : ` ${slice}`);
  }
  return chunks;
}

/** SDD CRM-024 — canned reply suggestions from ticket keywords. */
export function suggestReplies(ticket: TicketSnapshot): {
  title: string;
  body: string;
}[] {
  const text = `${ticket.subject} ${ticket.description || ''}`.toLowerCase();
  const out: { title: string; body: string }[] = [];
  if (/bill|invoice|payment|refund/.test(text)) {
    out.push({
      title: 'Billing acknowledgment',
      body: `Hello ${ticket.customerName}, thanks for reaching out about billing. I'm reviewing ${ticket.ticketNumber} and will confirm the charges shortly.`,
    });
  }
  if (/password|login|access|reset/.test(text)) {
    out.push({
      title: 'Access help',
      body: `Hi ${ticket.customerName}, please use Forgot password on the sign-in page, then check inbox/spam for the reset link. Reply here if that fails.`,
    });
  }
  if (/urgent|asap|outage|down/.test(text) || ticket.priority === 'Urgent') {
    out.push({
      title: 'Urgent triage',
      body: `We've marked ${ticket.ticketNumber} as high priority and are investigating now. I'll update you as soon as we have next steps.`,
    });
  }
  if (out.length === 0) {
    out.push({
      title: 'General acknowledgment',
      body: `Hello ${ticket.customerName}, thanks for contacting support about "${ticket.subject}". We're looking into ${ticket.ticketNumber} and will follow up soon.`,
    });
  }
  if (out.length < 2) {
    out.push({
      title: 'Request more detail',
      body: `To speed up ${ticket.ticketNumber}, could you share any screenshots, error codes, or account identifiers related to this issue?`,
    });
  }
  return out.slice(0, 3);
}

/** SDD CRM-025 — keyword category/priority. */
export function categorizeTicket(ticket: TicketSnapshot): {
  category: string;
  priority: string;
  confidence: number;
} {
  const text = `${ticket.subject} ${ticket.description || ''}`.toLowerCase();
  let category = 'General';
  let priority = 'Medium';
  let confidence = 0.55;
  if (/bill|invoice|payment|refund|charge/.test(text)) {
    category = 'Billing';
    confidence = 0.82;
  } else if (/password|login|api|token|whatsapp|sms|email|bug|error|crash/.test(text)) {
    category = 'Technical';
    confidence = 0.8;
  }
  if (/urgent|asap|outage|critical|down/.test(text)) {
    priority = 'Urgent';
    confidence = Math.max(confidence, 0.85);
  } else if (/high|important/.test(text)) {
    priority = 'High';
  } else if (/low|minor|question/.test(text)) {
    priority = 'Low';
  }
  return { category, priority, confidence };
}

/** SDD CRM-026 — FAQ-backed chatbot reply (optional prior turns for polish / 043). */
export function chatReply(
  message: string,
  faqs: { id: string; title: string }[],
  priorTurns: { role: 'user' | 'assistant'; text: string }[] = [],
): { reply: string; sources: { id: string; title: string }[] } {
  const priorUser = priorTurns
    .filter((t) => t.role === 'user')
    .map((t) => t.text)
    .join(' ');
  const q = `${priorUser} ${message}`.trim().toLowerCase();
  const hits = faqs.filter((f) => {
    const t = f.title.toLowerCase();
    return q.split(/\s+/).some((w) => w.length > 3 && t.includes(w));
  });
  if (hits.length > 0) {
    const top = hits.slice(0, 2);
    const followUp =
      priorTurns.length > 0
        ? ' (continuing from our earlier messages)'
        : '';
    return {
      reply: `Based on our FAQs${followUp}, you may find these helpful: ${top.map((h) => h.title).join('; ')}. If that doesn't resolve it, submit a portal request.`,
      sources: top,
    };
  }
  if (priorTurns.length > 0) {
    return {
      reply:
        'Still here from our earlier chat. Try asking about passwords, billing, or tickets — or open Submit a request from the portal home.',
      sources: [],
    };
  }
  return {
    reply:
      'I can help with common questions. Try asking about passwords, billing, or tickets — or open Submit a request from the portal home.',
    sources: [],
  };
}

/** SDD CRM-026 deferred / 047 — detect request for a human agent. */
export function wantsHumanHandoff(message: string): boolean {
  return /\b(human|agent|representative|person|speak to someone)\b/i.test(message.trim());
}
