import assert from 'node:assert/strict';
import { describe, it } from 'node:test';
import { chatReply } from '../src/infrastructure/ai/heuristic.provider';

describe('CRM-026 chatReply', () => {
  it('returns FAQ sources when titles match the question', () => {
    const row = chatReply('How do I reset my password?', [
      { id: 'f1', title: 'Password reset steps' },
      { id: 'f2', title: 'Billing FAQ' },
    ]);
    assert.match(row.reply, /password|FAQ|helpful/i);
    assert.ok(row.sources.some((s) => s.id === 'f1'));
  });

  it('falls back when no FAQ matches', () => {
    const row = chatReply('xyzzy', []);
    assert.ok(row.reply.length > 10);
    assert.equal(row.sources.length, 0);
  });
});
