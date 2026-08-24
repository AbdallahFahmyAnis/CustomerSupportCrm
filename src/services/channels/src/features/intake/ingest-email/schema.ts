import { IngestEmailBody } from './email.types';

/** SDD CRM-008 — email intake validation. */
export function validateEmailIngestInput(input: IngestEmailBody): string | null {
  const from = (input.from ?? input.email)?.trim().toLowerCase() ?? '';
  const subject = input.subject?.trim() ?? '';
  const body = (input.body ?? input.message)?.trim() ?? '';

  if (!from || !subject || !body) {
    return 'from, subject, and body are required.';
  }
  if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(from)) {
    return 'from must be a valid email address.';
  }
  return null;
}
