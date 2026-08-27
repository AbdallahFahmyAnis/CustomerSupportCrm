import { IngestSmsBody } from './sms.types';
import { normalizePhone } from '../../../infrastructure/phone/normalize-phone';

export { normalizePhone };

/** SDD CRM-011 — SMS intake validation. */
export function validateSmsIngestInput(input: IngestSmsBody): string | null {
  const from = normalizePhone(
    String(input.from ?? input.phone ?? input.msisdn ?? ''),
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
