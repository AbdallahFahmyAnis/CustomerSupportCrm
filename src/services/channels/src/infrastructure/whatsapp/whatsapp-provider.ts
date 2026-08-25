/** SDD CRM-040 — WhatsApp provider contract. */
export interface InboundWhatsAppPayload {
  from: string;
  body: string;
  name?: string;
  subject?: string;
}

export interface OutboundWhatsAppPayload {
  to: string;
  body: string;
  ticketId: string;
}

export interface WhatsAppProvider {
  parseInbound(raw: unknown): InboundWhatsAppPayload;
  sendOutbound(payload: OutboundWhatsAppPayload): Promise<void>;
}

export const WHATSAPP_PROVIDER = 'WHATSAPP_PROVIDER';
