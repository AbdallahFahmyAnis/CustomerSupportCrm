/** SDD CRM-008 — email intake use-case types. */
export class IngestEmailCommand {
  constructor(
    public readonly from: string,
    public readonly subject: string,
    public readonly body: string,
    public readonly name?: string,
  ) {}
}

export interface IngestEmailResult {
  requestId: string;
  ticketId: string;
  ticketNumber: string;
}

export interface IngestEmailBody {
  from?: string;
  email?: string;
  subject?: string;
  body?: string;
  message?: string;
  name?: string;
}
