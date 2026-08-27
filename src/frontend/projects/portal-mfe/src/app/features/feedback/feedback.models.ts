/** SDD CRM-030 */
export interface TicketFeedback {
  id: string;
  ticketId: string;
  rating: number;
  comment?: string | null;
  createdAt: string;
}

export interface SubmitFeedbackBody {
  ticketId?: string | null;
  ticketNumber?: string | null;
  rating: number;
  comment?: string | null;
}
