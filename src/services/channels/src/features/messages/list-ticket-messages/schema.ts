/** SDD CRM-012 — ticket id schema. */
export function requireTicketId(ticketId?: string): string {
  const value = ticketId?.trim() ?? '';
  if (!value) {
    throw new Error('ticketId is required.');
  }
  return value;
}
