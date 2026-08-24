/** SDD CRM-004…007 / specs/003-ticket-lifecycle */
export interface TicketSummary {
  id: string;
  ticketNumber: string;
  customerId: string;
  customerName: string;
  subject: string;
  category: string;
  priority: string;
  status: string;
  assignedAgentId?: string | null;
  assignedAgentName?: string | null;
  isEscalated: boolean;
}

export interface TicketHistory {
  id: string;
  field: string;
  oldValue?: string | null;
  newValue?: string | null;
  changedBy: string;
  changedAt: string;
}

export interface TicketDetail extends TicketSummary {
  description?: string | null;
  createdAt: string;
  updatedAt: string;
  history: TicketHistory[];
}

export interface TicketOptions {
  categories: string[];
  priorities: string[];
  statuses: string[];
  agents: { id: string; name: string }[];
}

export interface CustomerOption {
  id: string;
  displayName: string;
  uniqueIdentifier: string;
}

/** SDD CRM-008 — channel message on ticket thread. */
export interface ChannelMessageDto {
  id: string;
  ticketId: string;
  channel: string;
  direction: string;
  body: string;
  fromEmail?: string;
  createdAt: string;
}
