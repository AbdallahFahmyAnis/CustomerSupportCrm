/** SDD CRM-010 — live chat intake use-case types. */
export class IngestChatCommand {
  constructor(
    public readonly email: string,
    public readonly body: string,
    public readonly name?: string,
    public readonly subject?: string,
    public readonly ticketId?: string,
    public readonly sessionId?: string,
  ) {}
}

export interface IngestChatResult {
  requestId: string;
  ticketId: string;
  ticketNumber: string;
}

export interface IngestChatBody {
  email?: string;
  from?: string;
  body?: string;
  message?: string;
  text?: string;
  name?: string;
  subject?: string;
  ticketId?: string;
  sessionId?: string;
}
