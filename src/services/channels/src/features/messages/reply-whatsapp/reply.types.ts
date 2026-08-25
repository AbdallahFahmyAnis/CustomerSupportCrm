/** SDD CRM-009 — reply WhatsApp use-case types. */
export class ReplyWhatsAppCommand {
  constructor(
    public readonly ticketId: string,
    public readonly body: string,
    public readonly to?: string,
  ) {}
}

export interface ReplyWhatsAppResult {
  messageId: string;
  ticketId: string;
  to: string;
}

export interface ReplyWhatsAppBody {
  body?: string;
  to?: string;
}
