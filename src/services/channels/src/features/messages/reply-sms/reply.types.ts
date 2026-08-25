/** SDD CRM-011 — reply SMS use-case types. */
export class ReplySmsCommand {
  constructor(
    public readonly ticketId: string,
    public readonly body: string,
    public readonly to?: string,
  ) {}
}

export interface ReplySmsResult {
  messageId: string;
  ticketId: string;
  to: string;
}

export interface ReplySmsBody {
  body?: string;
  to?: string;
}
