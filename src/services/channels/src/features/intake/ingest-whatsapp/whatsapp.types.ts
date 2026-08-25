/** SDD CRM-009 — WhatsApp intake use-case types. */
export class IngestWhatsAppCommand {
  constructor(
    public readonly from: string,
    public readonly body: string,
    public readonly name?: string,
    public readonly subject?: string,
  ) {}
}

export interface IngestWhatsAppResult {
  requestId: string;
  ticketId: string;
  ticketNumber: string;
}

export interface IngestWhatsAppBody {
  from?: string;
  phone?: string;
  waId?: string;
  body?: string;
  message?: string;
  text?: string;
  name?: string;
  subject?: string;
}
