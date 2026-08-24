/** SDD CRM-027 / CRM-028 — portal request models. */
export interface SubmitRequestBody {
  name: string;
  email: string;
  subject: string;
  message: string;
}

export interface SubmitRequestResult {
  requestId: string;
  ticketId: string;
  ticketNumber: string;
}

export interface PortalRequestSummary {
  requestId: string;
  ticketId: string;
  ticketNumber: string;
  subject: string;
  status: string;
  createdAt: string;
}
