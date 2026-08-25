import assert from 'node:assert/strict';
import { describe, it } from 'node:test';
import { chatReply } from '../src/infrastructure/ai/heuristic.provider';
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

  it('stores and returns a stable sessionId with capped turns', () => {
    const store = new ChatSessionStore();
    const id = store.ensureSessionId();
    assert.ok(id.length > 8);
    assert.equal(store.ensureSessionId(id), id);

    for (let i = 0; i < 8; i++) {
      store.append(id, `user-${i}`, `bot-${i}`);
    }
    const turns = store.getTurns(id);
    assert.equal(turns.length, 12); // 6 turns × user+assistant
    assert.equal(turns[0].text, 'user-2');
  });
});
