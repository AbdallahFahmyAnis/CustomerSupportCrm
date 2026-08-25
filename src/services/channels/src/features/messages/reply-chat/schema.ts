import { ReplyChatBody } from './reply.types';

/** SDD CRM-010 — outbound live chat reply validation. */
export function validateReplyChatInput(
  ticketId: string,
  input: ReplyChatBody,
): string | null {
  if (!ticketId?.trim()) {
    return 'ticketId is required.';
  }
  const body = input.body?.trim() ?? '';
  if (!body) {
    return 'body is required.';
  }
  if (input.to != null && input.to.trim() && !input.to.includes('@')) {
    return 'to must be a valid email address.';
  }
  return null;
}
