import { IngestChatBody } from './chat.types';

/** SDD CRM-010 — live chat intake validation. */
export function validateChatIngestInput(input: IngestChatBody): string | null {
  const email = (input.email ?? input.from)?.trim().toLowerCase() ?? '';
  const body = (input.body ?? input.message ?? input.text)?.trim() ?? '';

  if (!email || !body) {
    return 'email and body are required.';
  }
  if (!email.includes('@')) {
    return 'email must be a valid address.';
  }
  return null;
}
