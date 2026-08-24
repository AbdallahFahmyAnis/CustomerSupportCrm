/** SDD CRM-012 — list inbound messages for a ticket. */
export class ListTicketMessagesQuery {
  constructor(public readonly ticketId: string) {}
}
