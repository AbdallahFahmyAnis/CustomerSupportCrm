/** SDD CRM-020 — require gateway user id header. */
export function requireUserId(raw: string | undefined): string {
  const id = (raw ?? '').trim();
  if (!id) {
    throw new Error('X-Crm-User-Id header is required.');
  }
  return id;
}
