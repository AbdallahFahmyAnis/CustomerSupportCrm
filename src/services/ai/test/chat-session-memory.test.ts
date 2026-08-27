import assert from 'node:assert/strict';
import { mkdtempSync, readFileSync, rmSync } from 'node:fs';
import { tmpdir } from 'node:os';
import { join } from 'node:path';
import { describe, it } from 'node:test';
import { chatReply, wantsHumanHandoff } from '../src/infrastructure/ai/heuristic.provider';
import { ChatSessionStore } from '../src/infrastructure/chat/chat-session.store';

describe('CRM-026 chat session memory', () => {
  it('uses prior user turns when matching FAQs on a short follow-up', () => {
    const faqs = [
      { id: 'f1', title: 'Password reset steps' },
      { id: 'f2', title: 'Billing FAQ' },
    ];
    const prior = [
      { role: 'user' as const, text: 'I need password help' },
      { role: 'assistant' as const, text: 'Try the FAQ.' },
    ];
    const row = chatReply('tell me more', faqs, prior);
    assert.match(row.reply, /continuing|Password|FAQ/i);
    assert.ok(row.sources.some((s) => s.id === 'f1'));
  });

  it('persists sessions to disk and detects human handoff', () => {
    assert.equal(wantsHumanHandoff('I want a human agent please'), true);
    assert.equal(wantsHumanHandoff('password reset'), false);

    const dir = mkdtempSync(join(tmpdir(), 'crm-chat-'));
    const file = join(dir, 'sessions.json');
    process.env.CHAT_SESSIONS_PATH = file;
    try {
      const store = new ChatSessionStore();
      const id = store.ensureSessionId();
      store.append(id, 'hello', 'hi');
      const raw = JSON.parse(readFileSync(file, 'utf8')) as Record<string, unknown>;
      assert.ok(raw[id]);

      const reloaded = new ChatSessionStore();
      assert.equal(reloaded.getTurns(id).length, 2);
    } finally {
      delete process.env.CHAT_SESSIONS_PATH;
      rmSync(dir, { recursive: true, force: true });
    }
  });
});
