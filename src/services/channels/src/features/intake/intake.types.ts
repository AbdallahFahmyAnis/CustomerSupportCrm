/** SDD CRM-012 / CRM-027 — intake use-case types. */
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

export interface SubmitWebFormBody {
  name?: string;
  email?: string;
  subject?: string;
  message?: string;
}
