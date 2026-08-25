/** SDD CRM-040 — email provider contract. */
export interface InboundEmailPayload {
  from: string;
  subject: string;
  body: string;
  name?: string;
}

export interface OutboundEmailPayload {
  to: string;
  subject: string;
  body: string;
  ticketId: string;
}

export interface EmailProvider {
  /** Normalize provider-specific payload into a CRM inbound email. */
  parseInbound(raw: unknown): InboundEmailPayload;

  /** Send outbound email (dev logs; SMTP when configured). */
  sendOutbound(payload: OutboundEmailPayload): Promise<void>;
}

export const EMAIL_PROVIDER = 'EMAIL_PROVIDER';
