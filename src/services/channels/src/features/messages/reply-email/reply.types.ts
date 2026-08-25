/** SDD CRM-040 — reply email use-case types. */
export class ReplyEmailCommand {
  constructor(
    public readonly ticketId: string,
    public readonly body: string,
    public readonly to?: string,
  ) {}
}

export interface ReplyEmailResult {
  messageId: string;
  ticketId: string;
  to: string;
}

export interface ReplyEmailBody {
  body?: string;
  to?: string;
}
