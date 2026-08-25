import * as fs from 'fs';
import * as path from 'path';
import { randomUUID } from 'crypto';
import {
  DEMO_AGENT_ID,
  NotificationKind,
  NotificationRecord,
} from '../../domain/notification';

/** SDD CRM-020 — JSON file inbox store. */
export class NotificationsStore {
  private readonly filePath: string;
  private items: NotificationRecord[] = [];
  private loaded = false;

  constructor(dataPath?: string) {
    const root =
      dataPath ??
      process.env.NOTIFICATIONS_DATA_PATH ??
      path.join(process.cwd(), 'data');
    fs.mkdirSync(root, { recursive: true });
    this.filePath = path.join(root, 'notifications-store.json');
  }

  ensureSeeded(): void {
    this.load();
    if (this.items.length > 0) {
      return;
    }

    const now = new Date().toISOString();
    this.items = [
      {
        id: randomUUID(),
        userId: DEMO_AGENT_ID,
        title: 'Ticket assigned to you',
        body: 'TKT-1001 Invoice mismatch was assigned to Demo Agent.',
        kind: 'assignment',
        href: '/agent/tickets',
        createdAt: now,
        readAt: null,
      },
      {
        id: randomUUID(),
        userId: DEMO_AGENT_ID,
        title: 'SLA first response at risk',
        body: 'An Urgent ticket is approaching the first-response due time.',
        kind: 'sla',
        href: '/agent/tickets',
        createdAt: now,
        readAt: null,
      },
      {
        id: randomUUID(),
        userId: DEMO_AGENT_ID,
        title: 'Welcome to CRM alerts',
        body: 'You will see assignment and SLA alerts here.',
        kind: 'system',
        createdAt: now,
        readAt: now,
      },
    ];
    this.persist();
  }

  listForUser(userId: string): NotificationRecord[] {
    this.load();
    return this.items
      .filter((n) => n.userId === userId)
      .sort((a, b) => {
        const aUnread = a.readAt ? 1 : 0;
        const bUnread = b.readAt ? 1 : 0;
        if (aUnread !== bUnread) {
          return aUnread - bUnread;
        }
        return b.createdAt.localeCompare(a.createdAt);
      });
  }

  unreadCount(userId: string): number {
    return this.listForUser(userId).filter((n) => !n.readAt).length;
  }

  markRead(userId: string, id: string): NotificationRecord | null {
    this.load();
    const row = this.items.find((n) => n.id === id && n.userId === userId);
    if (!row) {
      return null;
    }
    if (!row.readAt) {
      row.readAt = new Date().toISOString();
      this.persist();
    }
    return row;
  }

  /** SDD CRM-016 — create an inbox item (mention / producer). */
  create(input: {
    userId: string;
    title: string;
    body: string;
    kind: NotificationKind;
    href?: string;
  }): NotificationRecord {
    this.load();
    const row: NotificationRecord = {
      id: randomUUID(),
      userId: input.userId.trim(),
      title: input.title.trim(),
      body: input.body.trim(),
      kind: input.kind,
      href: input.href?.trim() || undefined,
      createdAt: new Date().toISOString(),
      readAt: null,
    };
    this.items.push(row);
    this.persist();
    return row;
  }

  private load(): void {
    if (this.loaded) {
      return;
    }
    this.loaded = true;
    if (!fs.existsSync(this.filePath)) {
      this.items = [];
      return;
    }
    try {
      const raw = fs.readFileSync(this.filePath, 'utf8');
      const parsed = JSON.parse(raw) as { items?: NotificationRecord[] };
      this.items = Array.isArray(parsed.items) ? parsed.items : [];
    } catch {
      this.items = [];
    }
  }

  private persist(): void {
    fs.writeFileSync(
      this.filePath,
      JSON.stringify({ items: this.items }, null, 2),
      'utf8',
    );
  }
}
