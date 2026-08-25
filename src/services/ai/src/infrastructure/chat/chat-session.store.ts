import { Injectable } from '@nestjs/common';
import { randomUUID } from 'node:crypto';
import { existsSync, mkdirSync, readFileSync, writeFileSync } from 'node:fs';
import { dirname, join } from 'node:path';

export type ChatTurn = { role: 'user' | 'assistant'; text: string };

/** SDD CRM-026 deferred / 047 — file-backed multi-turn memory (last ~6 turns). */
@Injectable()
export class ChatSessionStore {
  private readonly sessions = new Map<string, ChatTurn[]>();
  private readonly maxTurns = 6;
  private readonly filePath: string;

  constructor() {
    this.filePath =
      process.env.CHAT_SESSIONS_PATH?.trim() ||
      join(process.cwd(), 'data', 'chat-sessions.json');
    this.load();
  }

  ensureSessionId(sessionId?: string | null): string {
    const id = (sessionId || '').trim();
    return id || randomUUID();
  }

  getTurns(sessionId: string): ChatTurn[] {
    return [...(this.sessions.get(sessionId) ?? [])];
  }

  append(sessionId: string, userText: string, assistantText: string): void {
    const list = this.sessions.get(sessionId) ?? [];
    list.push({ role: 'user', text: userText }, { role: 'assistant', text: assistantText });
    while (list.length > this.maxTurns * 2) {
      list.shift();
    }
    this.sessions.set(sessionId, list);
    this.persist();
  }

  /** Test helper — clear all sessions. */
  clear(): void {
    this.sessions.clear();
    this.persist();
  }

  private load(): void {
    try {
      if (!existsSync(this.filePath)) return;
      const raw = JSON.parse(readFileSync(this.filePath, 'utf8')) as Record<string, ChatTurn[]>;
      for (const [id, turns] of Object.entries(raw)) {
        if (Array.isArray(turns)) this.sessions.set(id, turns);
      }
    } catch {
      // corrupt file — start empty
    }
  }

  private persist(): void {
    try {
      mkdirSync(dirname(this.filePath), { recursive: true });
      const obj: Record<string, ChatTurn[]> = {};
      for (const [id, turns] of this.sessions) {
        obj[id] = turns;
      }
      writeFileSync(this.filePath, JSON.stringify(obj, null, 2), 'utf8');
    } catch {
      // non-fatal
    }
  }
}
