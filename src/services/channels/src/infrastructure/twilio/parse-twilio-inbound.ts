/** SDD CRM-040 — map Twilio form fields to CRM ingest shapes. */
export function parseTwilioInboundForm(params: Record<string, unknown>): {
  from: string;
  body: string;
} {
  const fromRaw = String(params.From ?? params.from ?? '').trim();
  const body = String(params.Body ?? params.body ?? params.Text ?? '').trim();
  const from = fromRaw.replace(/^whatsapp:/i, '').trim();
  return { from, body };
}

export function flattenFormBody(body: unknown): Record<string, string> {
  if (!body || typeof body !== 'object') {
    return {};
  }
  const out: Record<string, string> = {};
  for (const [key, value] of Object.entries(body as Record<string, unknown>)) {
    if (value === undefined || value === null) {
      continue;
    }
    if (Array.isArray(value)) {
      out[key] = String(value[0] ?? '');
    } else {
      out[key] = String(value);
    }
  }
  return out;
}
