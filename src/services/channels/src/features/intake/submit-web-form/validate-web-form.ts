/** SDD CRM-012 — shared validation for web-form intake. */
export function validateWebFormInput(input: {
  name?: string;
  email?: string;
  subject?: string;
  message?: string;
}): string | null {
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
