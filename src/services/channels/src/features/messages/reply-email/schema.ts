import { ReplyEmailBody } from './reply.types';

/** SDD CRM-040 — outbound email reply validation. */
export function validateReplyEmailInput(
  ticketId: string,
  input: ReplyEmailBody,
): string | null {
  if (!ticketId?.trim()) {
    return 'ticketId is required.';
  }
  const body = input.body?.trim() ?? '';
  if (!body) {
    return 'body is required.';
  }
  if (input.to != null && input.to.trim()) {
    const to = input.to.trim().toLowerCase();
    if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(to)) {
      return 'to must be a valid email address.';
    }
  }
  return null;
}
