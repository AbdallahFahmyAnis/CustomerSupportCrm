/** SDD CRM-040 — email provider contract (no real SDK in this slice). */
export interface InboundEmailPayload {
  from: string;
  subject: string;
  body: string;
  name?: string;
}

export interface EmailProvider {
  /** Normalize provider-specific payload into a CRM inbound email. */
  parseInbound(raw: unknown): InboundEmailPayload;
}
