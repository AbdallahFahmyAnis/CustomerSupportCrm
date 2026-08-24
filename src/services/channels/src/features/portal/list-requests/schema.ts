/** SDD CRM-028 — query schema for track-by-email. */
export function requireEmail(email?: string): string {
  const value = email?.trim().toLowerCase() ?? '';
  if (!value) {
    throw new Error('email query parameter is required.');
  }
  return value;
}
