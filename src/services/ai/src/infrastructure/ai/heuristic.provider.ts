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

/** SDD CRM-026 — FAQ-backed chatbot reply. */
export function chatReply(
  message: string,
  faqs: { id: string; title: string }[],
): { reply: string; sources: { id: string; title: string }[] } {
  const q = message.trim().toLowerCase();
  const hits = faqs.filter((f) => {
    const t = f.title.toLowerCase();
    return q.split(/\s+/).some((w) => w.length > 3 && t.includes(w));
  });
  if (hits.length > 0) {
    const top = hits.slice(0, 2);
    return {
      reply: `Based on our FAQs, you may find these helpful: ${top.map((h) => h.title).join('; ')}. If that doesn't resolve it, submit a portal request.`,
      sources: top,
    };
  }
  return {
    reply:
      'I can help with common questions. Try asking about passwords, billing, or tickets — or open Submit a request from the portal home.',
    sources: [],
  };
}
