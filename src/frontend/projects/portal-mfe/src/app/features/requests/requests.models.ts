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

/** SDD CRM-010 / CRM-012 — channel message on ticket thread. */
export interface ChannelMessageDto {
  id: string;
  ticketId: string;
  channel: string;
  direction: string;
  body: string;
  fromEmail?: string;
  createdAt: string;
}
