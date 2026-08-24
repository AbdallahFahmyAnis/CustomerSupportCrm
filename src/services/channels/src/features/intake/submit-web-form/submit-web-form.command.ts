/** SDD CRM-012 / CRM-027 — web form intake command. */
export class SubmitWebFormCommand {
  constructor(
    public readonly name: string,
    public readonly email: string,
    public readonly subject: string,
    public readonly message: string,
  ) {}
}

export interface SubmitWebFormResult {
  requestId: string;
  ticketId: string;
  ticketNumber: string;
}
