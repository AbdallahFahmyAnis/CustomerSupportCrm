export interface AuditLogEntry {
  id: string;
  occurredAt: string;
  action: string;
  actorEmail: string | null;
  targetEmail: string | null;
  detail: string | null;
  success: boolean;
}
