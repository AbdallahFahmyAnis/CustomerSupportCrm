/** SDD CRM-010 — reply live chat use-case types. */
export class ReplyChatCommand {
  constructor(
    public readonly ticketId: string,
    public readonly body: string,
    public readonly to?: string,
  ) {}
}

export interface ReplyChatResult {
  messageId: string;
  ticketId: string;
  to: string;
}

export interface ReplyChatBody {
  body?: string;
  to?: string;
}
