import { ReplyWhatsAppBody } from './reply.types';
import { normalizePhone } from '../../intake/ingest-whatsapp/schema';

/** SDD CRM-009 — outbound WhatsApp reply validation. */
export function validateReplyWhatsAppInput(
  ticketId: string,
  input: ReplyWhatsAppBody,
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
