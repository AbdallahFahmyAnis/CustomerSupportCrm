/** SDD CRM-020 — in-app notification entity. */
export type NotificationKind = 'assignment' | 'sla' | 'system';

export interface NotificationRecord {
  id: string;
  userId: string;
  title: string;
  body: string;
  kind: NotificationKind;
  href?: string;
  createdAt: string;
  readAt: string | null;
}

export const DEMO_AGENT_ID = '11111111-1111-1111-1111-111111111111';
