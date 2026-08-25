import assert from 'node:assert/strict';
import { describe, it } from 'node:test';
import { suggestReplies } from '../src/infrastructure/ai/heuristic.provider';

describe('CRM-024 suggestReplies', () => {
  it('returns billing suggestion for invoice tickets', () => {
    const rows = suggestReplies({
      id: '1',
      ticketNumber: 'TKT-2',
      subject: 'Invoice refund',
      description: 'Wrong payment charged',
      category: 'Billing',
      priority: 'Medium',
      status: 'New',
      customerName: 'Beta',
    });
    assert.ok(rows.length >= 1);
    assert.ok(rows.some((r) => /billing|refund|payment/i.test(r.title + r.body)));
  });
});
