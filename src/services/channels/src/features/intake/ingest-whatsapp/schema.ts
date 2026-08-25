import { IngestWhatsAppBody } from './whatsapp.types';

/** Normalize to digits with optional leading +. */
export function normalizePhone(raw: string): string {
  const trimmed = raw.trim();
  const hasPlus = trimmed.startsWith('+');
  const digits = trimmed.replace(/\D/g, '');
  if (!digits) {
    return '';
  }
  return hasPlus || digits.length > 10 ? `+${digits}` : digits;
}

/** SDD CRM-009 — WhatsApp intake validation. */
export function validateWhatsAppIngestInput(
  input: IngestWhatsAppBody,
): string | null {
  const from = normalizePhone(
    String(input.from ?? input.phone ?? input.waId ?? ''),
  );
  const body = (input.body ?? input.message ?? input.text)?.trim() ?? '';

  if (!from || !body) {
    return 'from (phone) and body are required.';
  }
  if (from.replace(/\D/g, '').length < 8) {
    return 'from must be a valid phone number.';
  }
  return null;
}
