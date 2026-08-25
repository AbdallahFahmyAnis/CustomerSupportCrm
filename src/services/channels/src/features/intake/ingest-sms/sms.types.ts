/** SDD CRM-011 — SMS intake use-case types. */
export class IngestSmsCommand {
  constructor(
    public readonly from: string,
    public readonly body: string,
    public readonly name?: string,
    public readonly subject?: string,
  ) {}
}

export interface IngestSmsResult {
  requestId: string;
  ticketId: string;
  ticketNumber: string;
}

export interface IngestSmsBody {
  from?: string;
  phone?: string;
  msisdn?: string;
  body?: string;
  message?: string;
  text?: string;
  name?: string;
  subject?: string;
}
