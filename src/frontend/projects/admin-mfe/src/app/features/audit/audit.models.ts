export interface AuditLogEntry {
  id: string;
  occurredAt: string;
  action: string;
  actorEmail: string | null;
  targetEmail: string | null;
  detail: string | null;
  success: boolean;
  service?: string;
}

export interface AuditLogDetail extends AuditLogEntry {
  actorUserId: string | null;
  actorDisplayName: string | null;
  targetUserId: string | null;
  targetDisplayName: string | null;
}

/** SDD CRM-036 / specs/051 */
export interface AuditLogPage {
  items: AuditLogEntry[];
  total: number;
  skip: number;
  take: number;
}
