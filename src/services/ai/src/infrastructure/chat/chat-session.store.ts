import { Injectable } from '@nestjs/common';
import { randomUUID } from 'node:crypto';

export type ChatTurn = { role: 'user' | 'assistant'; text: string };

/** SDD CRM-026 polish / 043 — in-memory multi-turn memory (last ~6 turns). */
@Injectable()
export class ChatSessionStore {
  private readonly sessions = new Map<string, ChatTurn[]>();
  private readonly maxTurns = 6;

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
  }

  /** Test helper — clear all sessions. */
  clear(): void {
    this.sessions.clear();
  }
}
