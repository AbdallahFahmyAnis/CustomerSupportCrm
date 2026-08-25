/** SDD CRM-040 — SMS provider contract. */
export interface InboundSmsPayload {
  from: string;
  body: string;
  name?: string;
  subject?: string;
}

export interface OutboundSmsPayload {
  to: string;
  body: string;
  ticketId: string;
}

export interface SmsProvider {
  parseInbound(raw: unknown): InboundSmsPayload;
  sendOutbound(payload: OutboundSmsPayload): Promise<void>;
}

export const SMS_PROVIDER = 'SMS_PROVIDER';
