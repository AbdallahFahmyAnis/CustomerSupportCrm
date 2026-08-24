/** SDD CRM-028 — list portal requests by email. */
export class ListPortalRequestsQuery {
  constructor(public readonly email: string) {}
}

export interface PortalRequestDto {
  requestId: string;
  ticketId: string;
  ticketNumber: string;
  subject: string;
  status: string;
  createdAt: string;
}
