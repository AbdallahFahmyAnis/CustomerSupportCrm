import { SubmitWebFormBody } from '../intake.types';

/** SDD CRM-012 — web-form intake validation schema. */
export function validateWebFormInput(input: SubmitWebFormBody): string | null {
  const name = input.name?.trim() ?? '';
  const email = input.email?.trim().toLowerCase() ?? '';
  const subject = input.subject?.trim() ?? '';
  const message = input.message?.trim() ?? '';

  if (!name || !email || !subject || !message) {
    return 'name, email, subject, and message are required.';
  }
  if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
    return 'email must be a valid address.';
  }
  return null;
}
