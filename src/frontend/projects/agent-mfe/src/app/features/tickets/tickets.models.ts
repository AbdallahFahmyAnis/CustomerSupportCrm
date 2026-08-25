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

/** SDD CRM-016 — internal agent note. */
export interface TicketNote {
  id: string;
  body: string;
  authorName: string;
  authorUserId?: string | null;
  mentionedUserIds: string[];
  createdAt: string;
}

/** SDD CRM-014 — follow-up task. */
export interface TicketTask {
  id: string;
  ticketId: string;
  title: string;
  dueAt?: string | null;
  assigneeUserId?: string | null;
  assigneeName?: string | null;
  status: string;
  createdAt: string;
  updatedAt: string;
}

/** SDD CRM-015 — canned reply. */
export interface QuickReply {
  id: string;
  title: string;
  body: string;
}

export interface TicketDetail extends TicketSummary {
  description?: string | null;
  createdAt: string;
  updatedAt: string;
  history: TicketHistory[];
  notes?: TicketNote[];
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

/** SDD CRM-017 — SLA evaluation for ticket detail. */
export interface SlaEvaluation {
  priority: string;
  firstResponseMinutes: number;
  resolutionMinutes: number;
  firstResponseDueAt: string;
  resolutionDueAt: string;
  firstResponseBreached: boolean;
  resolutionBreached: boolean;
  asOf: string;
}
