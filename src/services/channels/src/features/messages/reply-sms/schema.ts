import { ReplySmsBody } from './reply.types';
import { normalizePhone } from '../../intake/ingest-sms/schema';

/** SDD CRM-011 — outbound SMS reply validation. */
export function validateReplySmsInput(
  ticketId: string,
  input: ReplySmsBody,
): string | null {
  if (!ticketId?.trim()) {
    return 'ticketId is required.';
  }
  const body = input.body?.trim() ?? '';
  if (!body) {
    return 'body is required.';
  }
  if (input.to != null && input.to.trim()) {
    const to = normalizePhone(input.to);
    if (to.replace(/\D/g, '').length < 8) {
      return 'to must be a valid phone number.';
    }
  }
  return null;
}
