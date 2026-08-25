/** SDD CRM-010 — live chat provider contract. */
export interface InboundChatPayload {
  email: string;
  body: string;
  name?: string;
  subject?: string;
  ticketId?: string;
  sessionId?: string;
}

export interface OutboundChatPayload {
  to: string;
  body: string;
  ticketId: string;
}

export interface ChatProvider {
  parseInbound(raw: unknown): InboundChatPayload;
  sendOutbound(payload: OutboundChatPayload): Promise<void>;
}

export const CHAT_PROVIDER = Symbol('CHAT_PROVIDER');
