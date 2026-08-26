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
  const continuing = priorTurns.length > 0;
  const followTag = continuing ? ' (continuing from our earlier messages)' : '';

  const hits = rankFaqHits(q, faqs);
  const topic = topicReply(q);

  if (hits.length > 0) {
    const top = hits.slice(0, 2);
    const titles = top.map((h) => h.title).join('; ');
    const lead = topic
      ? `${topic} Related FAQ${followTag}: ${titles}.`
      : `Based on our FAQs${followTag}, start with: ${titles}.`;
    return {
      reply: `${lead} Open the article under FAQs, or Submit a request / Live chat if you still need help.`,
      sources: top,
    };
  }

  if (topic) {
    return {
      reply: `${topic}${continuing ? ' Still here if you have a follow-up.' : ''} Or browse FAQs, Submit a request, or Live chat.`,
      sources: [],
    };
  }

  if (continuing) {
    return {
      reply:
        'Still here from our earlier chat. Ask about password reset, billing/invoices, tracking a ticket, live chat, or rating support — or say “human agent”.',
      sources: [],
    };
  }
  return {
    reply:
      'I can help with password reset, billing, tracking tickets, live chat, and feedback. Try one of those topics, browse FAQs, or open Submit a request.',
    sources: [],
  };
}

function rankFaqHits(
  q: string,
  faqs: { id: string; title: string }[],
): { id: string; title: string }[] {
  const words = q.split(/\s+/).filter((w) => w.length > 2);
  return faqs
    .map((f) => {
      const t = f.title.toLowerCase();
      let score = 0;
      for (const w of words) {
        if (t.includes(w)) score += w.length > 3 ? 2 : 1;
      }
      if (/password|reset|login|access/.test(q) && /password|reset|login|access/.test(t)) score += 3;
      if (/bill|invoice|payment|refund|charge/.test(q) && /bill|invoice|payment|refund/.test(t)) score += 3;
      if (/ticket|track|request|status/.test(q) && /track|request|ticket|status/.test(t)) score += 3;
      if (/chat|agent|live/.test(q) && /chat|live|agent/.test(t)) score += 3;
      if (/rate|feedback|csat|survey/.test(q) && /rate|feedback|support/.test(t)) score += 3;
      return { f, score };
    })
    .filter((x) => x.score > 0)
    .sort((a, b) => b.score - a.score)
    .map((x) => x.f);
}

function topicReply(q: string): string | null {
  if (/password|reset|login|sign[\s-]?in|access/.test(q)) {
    return 'To reset your password: open the portal sign-in page, choose Forgot password, and use the email link within 30 minutes (check spam).';
  }
  if (/bill|invoice|payment|refund|charge|po\b/.test(q)) {
    return 'For billing: include your invoice or PO number when you Submit a request or start Live chat so agents can match line items and tax.';
  }
  if (/track|ticket|status|request|open ticket/.test(q)) {
    return 'To track a request: open Track my requests. Signed-in customers see their tickets automatically; otherwise search with the email used when submitting.';
  }
  if (/live\s*chat|chat with|talk to support/.test(q)) {
    return 'Open Live chat from the portal menu. Signed-in customers skip name/email — send a message and we create a ticket.';
  }
  if (/rate|feedback|csat|survey|satisfied/.test(q)) {
    return 'After chat or when a ticket is Resolved/Closed, open Rate support (or use the link on Track / after Live chat) and leave a 1–5 rating.';
  }
  if (/faq|help center|knowledge/.test(q)) {
    return 'Browse FAQs in the portal for published answers, or ask me about passwords, billing, tickets, or chat.';
  }
  return null;
}

/** SDD CRM-026 deferred / 047 — detect request for a human agent. */
export function wantsHumanHandoff(message: string): boolean {
  return /\b(human|agent|representative|person|speak to someone)\b/i.test(message.trim());
}
