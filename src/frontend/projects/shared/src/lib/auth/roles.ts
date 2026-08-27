/** CRM role names — aligned with Identity RoleNames. */
export const CrmRoles = {
  Admin: 'Admin',
  Lead: 'Lead',
  Agent: 'Agent',
  Customer: 'Customer',
} as const;

export type CrmRole = (typeof CrmRoles)[keyof typeof CrmRoles];

export function normalizeRole(role: string | null | undefined): string {
  return (role ?? '').trim();
}

export function roleEquals(a: string | null | undefined, b: string): boolean {
  return normalizeRole(a).localeCompare(b, undefined, { sensitivity: 'base' }) === 0;
}

export function isAdminRole(role: string | null | undefined): boolean {
  return roleEquals(role, CrmRoles.Admin);
}

export function isCustomerRole(role: string | null | undefined): boolean {
  return roleEquals(role, CrmRoles.Customer);
}

/** Agent workspace (customers/tickets) — Admin, Lead, and Agent. */
export function canAccessAgentWorkspace(role: string | null | undefined): boolean {
  const r = normalizeRole(role);
  return (
    roleEquals(r, CrmRoles.Admin) ||
    roleEquals(r, CrmRoles.Lead) ||
    roleEquals(r, CrmRoles.Agent)
  );
}

/** Admin MFE — Admin only (matches backend AdminHttp). */
export function canAccessAdmin(role: string | null | undefined): boolean {
  return isAdminRole(role);
}

/** Customer portal — Customer clients (staff may also open /portal). */
export function canAccessCustomerPortal(role: string | null | undefined): boolean {
  return isCustomerRole(role) || canAccessAgentWorkspace(role);
}

export function homePathForRole(role: string | null | undefined): string {
  if (canAccessAdmin(role)) {
    return '/admin';
  }
  if (isCustomerRole(role)) {
    return '/portal';
  }
  return '/agent';
}
